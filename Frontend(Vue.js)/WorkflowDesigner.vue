<template>
  <div class="workflow-designer">
    <!-- Header -->
    <header class="designer-header">
      <div class="header-left">
        <h1>Visual AI Workflow Designer</h1>
        <p class="subtitle">Build powerful AI workflows without coding</p>
      </div>
      <div class="header-right">
        <button @click="showTemplates = true" class="btn btn-outline">
          <i class="fas fa-layer-group"></i>
          Templates
        </button>
        <button @click="createNewWorkflow" class="btn btn-primary">
          <i class="fas fa-plus"></i>
          New Workflow
        </button>
      </div>
    </header>

    <!-- Main Content -->
    <div class="designer-content">
      <!-- Workflow List Sidebar -->
      <aside class="workflows-sidebar" :class="{ collapsed: sidebarCollapsed }">
        <div class="sidebar-header">
          <h3>My Workflows</h3>
          <button @click="sidebarCollapsed = !sidebarCollapsed" class="collapse-btn">
            <i :class="sidebarCollapsed ? 'fas fa-chevron-right' : 'fas fa-chevron-left'"></i>
          </button>
        </div>
        
        <div v-if="!sidebarCollapsed" class="workflows-list">
          <div class="search-box">
            <input 
              type="text" 
              v-model="searchQuery" 
              placeholder="Search workflows..."
              class="search-input"
            />
            <i class="fas fa-search search-icon"></i>
          </div>
          
          <div class="workflow-filters">
            <button 
              v-for="filter in filters"
              :key="filter.key"
              @click="activeFilter = filter.key"
              :class="{ active: activeFilter === filter.key }"
              class="filter-btn"
            >
              {{ filter.label }}
            </button>
          </div>
          
          <div class="workflow-items">
            <div 
              v-for="workflow in filteredWorkflows"
              :key="workflow.id"
              @click="openWorkflow(workflow)"
              :class="{ active: currentWorkflow?.id === workflow.id }"
              class="workflow-item"
            >
              <div class="workflow-info">
                <h4>{{ workflow.name }}</h4>
                <p>{{ workflow.description || 'No description' }}</p>
                <div class="workflow-meta">
                  <span class="updated-date">
                    <i class="fas fa-clock"></i>
                    {{ formatDate(workflow.updatedAt) }}
                  </span>
                  <span class="node-count">
                    <i class="fas fa-sitemap"></i>
                    {{ getNodeCount(workflow) }} nodes
                  </span>
                </div>
              </div>
              <div class="workflow-actions">
                <button @click.stop="duplicateWorkflow(workflow)" class="action-btn" title="Duplicate">
                  <i class="fas fa-copy"></i>
                </button>
                <button @click.stop="deleteWorkflow(workflow.id)" class="action-btn delete" title="Delete">
                  <i class="fas fa-trash"></i>
                </button>
              </div>
            </div>
            
            <div v-if="filteredWorkflows.length === 0" class="empty-state">
              <i class="fas fa-robot"></i>
              <h3>No workflows found</h3>
              <p>Create your first AI workflow to get started!</p>
              <button @click="createNewWorkflow" class="btn btn-primary">
                Create Workflow
              </button>
            </div>
          </div>
        </div>
      </aside>

      <!-- Main Editor Area -->
      <main class="editor-area">
        <div v-if="currentWorkflow" class="editor-container">
          <WorkflowEditor 
            :workflow-id="currentWorkflow.id"
            @workflow-saved="onWorkflowSaved"
          />
        </div>
        
        <div v-else class="welcome-screen">
          <div class="welcome-content">
            <div class="welcome-icon">
              <i class="fas fa-brain"></i>
            </div>
            <h2>Welcome to Visual AI Workflow Designer</h2>
            <p>Create powerful AI-driven workflows using our intuitive drag-and-drop interface.</p>
            
            <div class="welcome-features">
              <div class="feature">
                <i class="fas fa-robot"></i>
                <h3>AI Components</h3>
                <p>Pre-built AI components for text generation, image analysis, speech recognition, and more.</p>
              </div>
              <div class="feature">
                <i class="fas fa-bolt"></i>
                <h3>Automation</h3>
                <p>Connect with Zapier, Power Automate, Slack, and email to automate your processes.</p>
              </div>
              <div class="feature">
                <i class="fas fa-code-branch"></i>
                <h3>Logic & Control</h3>
                <p>Add conditional logic, loops, and data transformation to create complex workflows.</p>
              </div>
            </div>
            
            <div class="welcome-actions">
              <button @click="createNewWorkflow" class="btn btn-primary btn-large">
                <i class="fas fa-plus"></i>
                Create Your First Workflow
              </button>
              <button @click="showTemplates = true" class="btn btn-outline btn-large">
                <i class="fas fa-layer-group"></i>
                Browse Templates
              </button>
            </div>
          </div>
        </div>
      </main>
    </div>

    <!-- Templates Modal -->
    <div v-if="showTemplates" class="modal-overlay" @click="showTemplates = false">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h2>Workflow Templates</h2>
          <button @click="showTemplates = false" class="close-btn">&times;</button>
        </div>
        
        <div class="templates-grid">
          <div 
            v-for="template in workflowTemplates"
            :key="template.id"
            @click="createFromTemplate(template)"
            class="template-card"
          >
            <div class="template-icon">
              <i :class="template.icon"></i>
            </div>
            <h3>{{ template.name }}</h3>
            <p>{{ template.description }}</p>
            <div class="template-tags">
              <span v-for="tag in template.tags" :key="tag" class="tag">{{ tag }}</span>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Loading Overlay -->
    <div v-if="loading" class="loading-overlay">
      <div class="loading-spinner">
        <i class="fas fa-spinner fa-spin"></i>
        <p>{{ loadingMessage }}</p>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, computed, onMounted } from 'vue'
import WorkflowEditor from './WorkflowEditor.vue'

export default {
  name: 'WorkflowDesigner',
  components: {
    WorkflowEditor
  },
  setup() {
    const workflows = ref([])
    const currentWorkflow = ref(null)
    const searchQuery = ref('')
    const activeFilter = ref('all')
    const sidebarCollapsed = ref(false)
    const showTemplates = ref(false)
    const loading = ref(false)
    const loadingMessage = ref('')
    
    const filters = [
      { key: 'all', label: 'All' },
      { key: 'recent', label: 'Recent' },
      { key: 'ai', label: 'AI Workflows' },
      { key: 'automation', label: 'Automation' }
    ]
    
    const workflowTemplates = [
      {
        id: 'customer-support',
        name: 'AI Customer Support',
        description: 'Automated customer support with sentiment analysis and smart routing',
        icon: 'fas fa-headset',
        tags: ['AI', 'Customer Service', 'Automation'],
        definition: {
          nodes: [
            { type: 'data-input', name: 'Customer Message' },
            { type: 'ai-sentiment', name: 'Analyze Sentiment' },
            { type: 'logic-condition', name: 'Route by Sentiment' },
            { type: 'ai-text', name: 'Generate Response' },
            { type: 'automation-email', name: 'Send Response' }
          ]
        }
      },
      {
        id: 'content-moderation',
        name: 'Content Moderation',
        description: 'Automatically moderate user-generated content using AI',
        icon: 'fas fa-shield-alt',
        tags: ['AI', 'Moderation', 'Safety'],
        definition: {
          nodes: [
            { type: 'data-input', name: 'User Content' },
            { type: 'ai-text', name: 'Content Analysis' },
            { type: 'logic-condition', name: 'Safety Check' },
            { type: 'automation-slack', name: 'Alert Moderators' }
          ]
        }
      },
      {
        id: 'document-processing',
        name: 'Document Processing',
        description: 'Extract and process information from documents',
        icon: 'fas fa-file-alt',
        tags: ['AI', 'OCR', 'Documents'],
        definition: {
          nodes: [
            { type: 'data-input', name: 'Document Upload' },
            { type: 'ai-image', name: 'OCR Extraction' },
            { type: 'ai-text', name: 'Information Extraction' },
            { type: 'data-transform', name: 'Format Data' },
            { type: 'data-output', name: 'Results' }
          ]
        }
      },
      {
        id: 'social-media-monitor',
        name: 'Social Media Monitor',
        description: 'Monitor social media mentions and respond automatically',
        icon: 'fas fa-hashtag',
        tags: ['Social Media', 'Monitoring', 'AI'],
        definition: {
          nodes: [
            { type: 'automation-zapier', name: 'Social Media Trigger' },
            { type: 'ai-sentiment', name: 'Analyze Mention' },
            { type: 'logic-condition', name: 'Filter Important' },
            { type: 'automation-slack', name: 'Notify Team' }
          ]
        }
      }
    ]
    
    const filteredWorkflows = computed(() => {
      let result = workflows.value
      
      // Apply search filter
      if (searchQuery.value) {
        const query = searchQuery.value.toLowerCase()
        result = result.filter(w => 
          w.name.toLowerCase().includes(query) ||
          w.description?.toLowerCase().includes(query)
        )
      }
      
      // Apply category filter
      switch (activeFilter.value) {
        case 'recent':
          result = result.slice().sort((a, b) => new Date(b.updatedAt) - new Date(a.updatedAt)).slice(0, 10)
          break
        case 'ai':
          result = result.filter(w => {
            const def = JSON.parse(w.definition || '{}')
            return def.nodes?.some(n => n.type?.startsWith('ai'))
          })
          break
        case 'automation':
          result = result.filter(w => {
            const def = JSON.parse(w.definition || '{}')
            return def.nodes?.some(n => n.type?.startsWith('automation'))
          })
          break
      }
      
      return result
    })
    
    const loadWorkflows = async () => {
      try {
        loading.value = true
        loadingMessage.value = 'Loading workflows...'
        
        const response = await fetch('/api/workflow')
        if (response.ok) {
          workflows.value = await response.json()
        }
      } catch (error) {
        console.error('Failed to load workflows:', error)
      } finally {
        loading.value = false
      }
    }
    
    const createNewWorkflow = () => {
      const newWorkflow = {
        id: null,
        name: 'New Workflow',
        description: '',
        definition: JSON.stringify({ nodes: [], edges: [] }),
        createdAt: new Date().toISOString(),
        updatedAt: new Date().toISOString()
      }
      
      currentWorkflow.value = newWorkflow
    }
    
    const openWorkflow = (workflow) => {
      currentWorkflow.value = workflow
    }
    
    const createFromTemplate = async (template) => {
      try {
        loading.value = true
        loadingMessage.value = 'Creating workflow from template...'
        
        const newWorkflow = {
          name: template.name,
          description: template.description,
          definition: JSON.stringify(template.definition)
        }
        
        const response = await fetch('/api/workflow', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(newWorkflow)
        })
        
        if (response.ok) {
          const createdWorkflow = await response.json()
          workflows.value.unshift(createdWorkflow)
          currentWorkflow.value = createdWorkflow
          showTemplates.value = false
        }
      } catch (error) {
        console.error('Failed to create workflow from template:', error)
      } finally {
        loading.value = false
      }
    }
    
    const duplicateWorkflow = async (workflow) => {
      try {
        loading.value = true
        loadingMessage.value = 'Duplicating workflow...'
        
        const duplicated = {
          name: `${workflow.name} (Copy)`,
          description: workflow.description,
          definition: workflow.definition
        }
        
        const response = await fetch('/api/workflow', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(duplicated)
        })
        
        if (response.ok) {
          const newWorkflow = await response.json()
          workflows.value.unshift(newWorkflow)
        }
      } catch (error) {
        console.error('Failed to duplicate workflow:', error)
      } finally {
        loading.value = false
      }
    }
    
    const deleteWorkflow = async (workflowId) => {
      if (!confirm('Are you sure you want to delete this workflow?')) {
        return
      }
      
      try {
        const response = await fetch(`/api/workflow/${workflowId}`, {
          method: 'DELETE'
        })
        
        if (response.ok) {
          workflows.value = workflows.value.filter(w => w.id !== workflowId)
          if (currentWorkflow.value?.id === workflowId) {
            currentWorkflow.value = null
          }
        }
      } catch (error) {
        console.error('Failed to delete workflow:', error)
      }
    }
    
    const onWorkflowSaved = (savedWorkflow) => {
      const index = workflows.value.findIndex(w => w.id === savedWorkflow.id)
      if (index !== -1) {
        workflows.value[index] = savedWorkflow
      } else {
        workflows.value.unshift(savedWorkflow)
      }
      currentWorkflow.value = savedWorkflow
    }
    
    const formatDate = (dateString) => {
      const date = new Date(dateString)
      const now = new Date()
      const diffMs = now - date
      const diffDays = Math.floor(diffMs / (1000 * 60 * 60 * 24))
      
      if (diffDays === 0) return 'Today'
      if (diffDays === 1) return 'Yesterday'
      if (diffDays < 7) return `${diffDays} days ago`
      
      return date.toLocaleDateString()
    }
    
    const getNodeCount = (workflow) => {
      try {
        const definition = JSON.parse(workflow.definition || '{}')
        return definition.nodes?.length || 0
      } catch {
        return 0
      }
    }
    
    onMounted(() => {
      loadWorkflows()
    })
    
    return {
      workflows,
      currentWorkflow,
      searchQuery,
      activeFilter,
      sidebarCollapsed,
      showTemplates,
      loading,
      loadingMessage,
      filters,
      workflowTemplates,
      filteredWorkflows,
      createNewWorkflow,
      openWorkflow,
      createFromTemplate,
      duplicateWorkflow,
      deleteWorkflow,
      onWorkflowSaved,
      formatDate,
      getNodeCount
    }
  }
}
</script>

<style scoped>
.workflow-designer {
  height: 100vh;
  display: flex;
  flex-direction: column;
  background: #f8f9fa;
}

.designer-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 2rem;
  background: white;
  border-bottom: 1px solid #dee2e6;
  box-shadow: 0 2px 4px rgba(0,0,0,0.1);
}

.header-left h1 {
  margin: 0;
  color: #212529;
  font-size: 1.5rem;
  font-weight: 600;
}

.subtitle {
  margin: 0;
  color: #6c757d;
  font-size: 0.9rem;
}

.header-right {
  display: flex;
  gap: 1rem;
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 0.5rem;
  transition: all 0.2s ease;
}

.btn-primary {
  background: #007bff;
  color: white;
}

.btn-primary:hover {
  background: #0056b3;
}

.btn-outline {
  background: transparent;
  color: #007bff;
  border: 1px solid #007bff;
}

.btn-outline:hover {
  background: #007bff;
  color: white;
}

.btn-large {
  padding: 0.75rem 1.5rem;
  font-size: 1rem;
}

.designer-content {
  flex: 1;
  display: flex;
  overflow: hidden;
}

.workflows-sidebar {
  width: 350px;
  background: white;
  border-right: 1px solid #dee2e6;
  display: flex;
  flex-direction: column;
  transition: width 0.3s ease;
}

.workflows-sidebar.collapsed {
  width: 50px;
}

.sidebar-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  border-bottom: 1px solid #dee2e6;
}

.sidebar-header h3 {
  margin: 0;
  font-size: 1.1rem;
  color: #495057;
}

.collapse-btn {
  background: none;
  border: none;
  cursor: pointer;
  color: #6c757d;
  padding: 0.5rem;
  border-radius: 4px;
  transition: background-color 0.2s ease;
}

.collapse-btn:hover {
  background: #f8f9fa;
}

.workflows-list {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.search-box {
  position: relative;
  margin: 1rem;
}

.search-input {
  width: 100%;
  padding: 0.5rem 2.5rem 0.5rem 1rem;
  border: 1px solid #ced4da;
  border-radius: 6px;
  font-size: 0.9rem;
}

.search-icon {
  position: absolute;
  right: 1rem;
  top: 50%;
  transform: translateY(-50%);
  color: #6c757d;
}

.workflow-filters {
  display: flex;
  padding: 0 1rem;
  gap: 0.5rem;
  margin-bottom: 1rem;
}

.filter-btn {
  padding: 0.5rem 1rem;
  border: 1px solid #dee2e6;
  background: white;
  border-radius: 20px;
  cursor: pointer;
  font-size: 0.8rem;
  transition: all 0.2s ease;
}

.filter-btn.active,
.filter-btn:hover {
  background: #007bff;
  color: white;
  border-color: #007bff;
}

.workflow-items {
  flex: 1;
  overflow-y: auto;
  padding: 0 1rem;
}

.workflow-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  margin-bottom: 0.5rem;
  background: white;
  border: 1px solid #e9ecef;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s ease;
}

.workflow-item:hover {
  border-color: #007bff;
  box-shadow: 0 2px 8px rgba(0,123,255,0.1);
}

.workflow-item.active {
  border-color: #007bff;
  background: #f8f9ff;
}

.workflow-info h4 {
  margin: 0 0 0.5rem 0;
  font-size: 0.95rem;
  color: #212529;
}

.workflow-info p {
  margin: 0 0 0.5rem 0;
  font-size: 0.8rem;
  color: #6c757d;
}

.workflow-meta {
  display: flex;
  gap: 1rem;
  font-size: 0.75rem;
  color: #868e96;
}

.workflow-actions {
  display: flex;
  gap: 0.5rem;
}

.action-btn {
  background: none;
  border: none;
  padding: 0.5rem;
  border-radius: 4px;
  cursor: pointer;
  color: #6c757d;
  transition: all 0.2s ease;
}

.action-btn:hover {
  background: #f8f9fa;
  color: #495057;
}

.action-btn.delete:hover {
  background: #f8d7da;
  color: #721c24;
}

.editor-area {
  flex: 1;
  display: flex;
  flex-direction: column;
}

.editor-container {
  flex: 1;
}

.welcome-screen {
  flex: 1;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem;
}

.welcome-content {
  text-align: center;
  max-width: 800px;
}

.welcome-icon {
  font-size: 4rem;
  color: #007bff;
  margin-bottom: 2rem;
}

.welcome-content h2 {
  margin: 0 0 1rem 0;
  color: #212529;
  font-size: 2rem;
}

.welcome-content p {
  margin: 0 0 3rem 0;
  color: #6c757d;
  font-size: 1.1rem;
}

.welcome-features {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 2rem;
  margin-bottom: 3rem;
}

.feature {
  text-align: center;
}

.feature i {
  font-size: 2rem;
  color: #007bff;
  margin-bottom: 1rem;
}

.feature h3 {
  margin: 0 0 0.5rem 0;
  color: #212529;
}

.feature p {
  margin: 0;
  color: #6c757d;
  font-size: 0.9rem;
}

.welcome-actions {
  display: flex;
  justify-content: center;
  gap: 1rem;
}

.empty-state {
  text-align: center;
  padding: 3rem 1rem;
  color: #6c757d;
}

.empty-state i {
  font-size: 3rem;
  margin-bottom: 1rem;
  color: #dee2e6;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0,0,0,0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  border-radius: 12px;
  max-width: 800px;
  max-height: 80vh;
  overflow-y: auto;
  box-shadow: 0 20px 60px rgba(0,0,0,0.2);
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1.5rem;
  border-bottom: 1px solid #dee2e6;
}

.modal-header h2 {
  margin: 0;
  color: #212529;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #6c757d;
  padding: 0.5rem;
}

.templates-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  padding: 1.5rem;
}

.template-card {
  padding: 1.5rem;
  border: 1px solid #dee2e6;
  border-radius: 8px;
  cursor: pointer;
  transition: all 0.2s ease;
  text-align: center;
}

.template-card:hover {
  border-color: #007bff;
  box-shadow: 0 4px 12px rgba(0,123,255,0.1);
}

.template-icon {
  font-size: 2rem;
  color: #007bff;
  margin-bottom: 1rem;
}

.template-card h3 {
  margin: 0 0 0.5rem 0;
  color: #212529;
}

.template-card p {
  margin: 0 0 1rem 0;
  color: #6c757d;
  font-size: 0.9rem;
}

.template-tags {
  display: flex;
  flex-wrap: wrap;
  gap: 0.5rem;
  justify-content: center;
}

.tag {
  padding: 0.25rem 0.5rem;
  background: #f8f9fa;
  border: 1px solid #dee2e6;
  border-radius: 12px;
  font-size: 0.75rem;
  color: #495057;
}

.loading-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(255,255,255,0.9);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 2000;
}

.loading-spinner {
  text-align: center;
  color: #007bff;
}

.loading-spinner i {
  font-size: 2rem;
  margin-bottom: 1rem;
}

@media (max-width: 768px) {
  .workflows-sidebar {
    width: 100%;
    max-width: 300px;
  }
  
  .welcome-features {
    grid-template-columns: 1fr;
  }
  
  .welcome-actions {
    flex-direction: column;
    align-items: center;
  }
}
</style>