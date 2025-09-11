<template>
  <div class="workflow-editor">
    <div class="editor-header">
      <h2>{{ workflow.name || 'New Workflow' }}</h2>
      <div class="editor-actions">
        <button @click="saveWorkflow" class="btn btn-primary">Save</button>
        <button @click="runWorkflow" class="btn btn-success">Run</button>
        <button @click="showNodeLibrary = !showNodeLibrary" class="btn btn-secondary">
          {{ showNodeLibrary ? 'Hide' : 'Show' }} Library
        </button>
      </div>
    </div>
    
    <div class="editor-content">
      <!-- Node Library Sidebar -->
      <div v-if="showNodeLibrary" class="node-library">
        <h3>AI Components</h3>
        <AiComponentLibrary @add-node="addNode" />
      </div>
      
      <!-- Workflow Canvas -->
      <div class="workflow-canvas" ref="canvas" @drop="onDrop" @dragover.prevent>
        <WorkflowNode
          v-for="node in workflow.nodes"
          :key="node.id"
          :node="node"
          :selected="selectedNode?.id === node.id"
          @select="selectNode"
          @update="updateNode"
          @delete="deleteNode"
          @connect="startConnection"
        />
        
        <WorkflowEdge
          v-for="edge in workflow.edges"
          :key="edge.id"
          :edge="edge"
          @delete="deleteEdge"
        />
        
        <!-- Connection Line (while dragging) -->
        <svg v-if="connectionDrag" class="connection-overlay">
          <line
            :x1="connectionDrag.startX"
            :y1="connectionDrag.startY"
            :x2="connectionDrag.currentX"
            :y2="connectionDrag.currentY"
            stroke="#007bff"
            stroke-width="2"
            stroke-dasharray="5,5"
          />
        </svg>
      </div>
      
      <!-- Properties Panel -->
      <div v-if="selectedNode" class="properties-panel">
        <h3>Node Properties</h3>
        <div class="property-group">
          <label>Name:</label>
          <input v-model="selectedNode.name" @input="updateSelectedNode" />
        </div>
        
        <div class="property-group">
          <label>Description:</label>
          <textarea v-model="selectedNode.description" @input="updateSelectedNode"></textarea>
        </div>
        
        <!-- Dynamic properties based on node type -->
        <AiComponent
          v-if="selectedNode.type.startsWith('ai')"
          :node="selectedNode"
          @update="updateSelectedNode"
        />
      </div>
    </div>
    
    <!-- Execution Status -->
    <div v-if="executionStatus" class="execution-status">
      <div class="status-header">
        <h4>Execution Status: {{ executionStatus.status }}</h4>
        <button @click="executionStatus = null" class="close-btn">&times;</button>
      </div>
      <div class="status-content">
        <p v-if="executionStatus.status === 'running'">Workflow is running...</p>
        <div v-else-if="executionStatus.status === 'completed'">
          <h5>Results:</h5>
          <pre>{{ JSON.stringify(executionStatus.output, null, 2) }}</pre>
        </div>
        <div v-else-if="executionStatus.status === 'failed'">
          <h5>Error:</h5>
          <p class="error">{{ executionStatus.error }}</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, reactive, onMounted, nextTick } from 'vue'
import WorkflowNode from './WorkflowNode.vue'
import WorkflowEdge from './WorkflowEdge.vue'
import AiComponentLibrary from '../utils/AiComponentLibrary.js'
import AiComponent from './AiComponent.vue'

export default {
  name: 'WorkflowEditor',
  components: {
    WorkflowNode,
    WorkflowEdge,
    AiComponentLibrary,
    AiComponent
  },
  props: {
    workflowId: {
      type: Number,
      default: null
    }
  },
  setup(props) {
    const workflow = reactive({
      id: props.workflowId,
      name: '',
      description: '',
      nodes: [],
      edges: []
    })
    
    const selectedNode = ref(null)
    const showNodeLibrary = ref(true)
    const connectionDrag = ref(null)
    const executionStatus = ref(null)
    const canvas = ref(null)
    
    const loadWorkflow = async () => {
      if (props.workflowId) {
        try {
          const response = await fetch(`/api/workflow/${props.workflowId}`)
          const data = await response.json()
          Object.assign(workflow, data)
        } catch (error) {
          console.error('Failed to load workflow:', error)
        }
      }
    }
    
    const saveWorkflow = async () => {
      try {
        const payload = {
          name: workflow.name,
          description: workflow.description,
          definition: JSON.stringify({
            nodes: workflow.nodes,
            edges: workflow.edges
          })
        }
        
        let response
        if (workflow.id) {
          response = await fetch(`/api/workflow/${workflow.id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
          })
        } else {
          response = await fetch('/api/workflow', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(payload)
          })
        }
        
        if (response.ok) {
          const savedWorkflow = await response.json()
          workflow.id = savedWorkflow.id
          alert('Workflow saved successfully!')
        }
      } catch (error) {
        console.error('Failed to save workflow:', error)
        alert('Failed to save workflow')
      }
    }
    
    const runWorkflow = async () => {
      if (!workflow.id) {
        alert('Please save the workflow first')
        return
      }
      
      try {
        executionStatus.value = { status: 'running' }
        
        const response = await fetch(`/api/workflow/${workflow.id}/execute`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ input: '{}' })
        })
        
        if (response.ok) {
          const execution = await response.json()
          executionStatus.value = {
            status: execution.status.toLowerCase(),
            output: JSON.parse(execution.output),
            error: execution.errorMessage
          }
        }
      } catch (error) {
        console.error('Failed to run workflow:', error)
        executionStatus.value = { status: 'failed', error: error.message }
      }
    }
    
    const addNode = (nodeType) => {
      const newNode = {
        id: Date.now().toString(),
        type: nodeType.type,
        name: nodeType.name,
        description: nodeType.description,
        x: 100,
        y: 100,
        inputs: nodeType.inputs || [],
        outputs: nodeType.outputs || [],
        properties: { ...nodeType.defaultProperties }
      }
      
      workflow.nodes.push(newNode)
    }
    
    const selectNode = (node) => {
      selectedNode.value = node
    }
    
    const updateNode = (updatedNode) => {
      const index = workflow.nodes.findIndex(n => n.id === updatedNode.id)
      if (index !== -1) {
        workflow.nodes[index] = { ...updatedNode }
      }
    }
    
    const updateSelectedNode = () => {
      if (selectedNode.value) {
        updateNode(selectedNode.value)
      }
    }
    
    const deleteNode = (nodeId) => {
      workflow.nodes = workflow.nodes.filter(n => n.id !== nodeId)
      workflow.edges = workflow.edges.filter(e => e.sourceNodeId !== nodeId && e.targetNodeId !== nodeId)
      if (selectedNode.value?.id === nodeId) {
        selectedNode.value = null
      }
    }
    
    const deleteEdge = (edgeId) => {
      workflow.edges = workflow.edges.filter(e => e.id !== edgeId)
    }
    
    const startConnection = (connectionInfo) => {
      connectionDrag.value = {
        sourceNodeId: connectionInfo.nodeId,
        sourcePortId: connectionInfo.portId,
        startX: connectionInfo.x,
        startY: connectionInfo.y,
        currentX: connectionInfo.x,
        currentY: connectionInfo.y
      }
      
      document.addEventListener('mousemove', onConnectionDrag)
      document.addEventListener('mouseup', onConnectionEnd)
    }
    
    const onConnectionDrag = (event) => {
      if (connectionDrag.value) {
        const rect = canvas.value.getBoundingClientRect()
        connectionDrag.value.currentX = event.clientX - rect.left
        connectionDrag.value.currentY = event.clientY - rect.top
      }
    }
    
    const onConnectionEnd = (event) => {
      document.removeEventListener('mousemove', onConnectionDrag)
      document.removeEventListener('mouseup', onConnectionEnd)
      
      // Check if we're over a valid target port
      const targetElement = document.elementFromPoint(event.clientX, event.clientY)
      const portElement = targetElement?.closest('.node-port.input')
      
      if (portElement && connectionDrag.value) {
        const targetNodeId = portElement.getAttribute('data-node-id')
        const targetPortId = portElement.getAttribute('data-port-id')
        
        if (targetNodeId !== connectionDrag.value.sourceNodeId) {
          const newEdge = {
            id: Date.now().toString(),
            sourceNodeId: connectionDrag.value.sourceNodeId,
            sourcePortId: connectionDrag.value.sourcePortId,
            targetNodeId,
            targetPortId
          }
          
          workflow.edges.push(newEdge)
        }
      }
      
      connectionDrag.value = null
    }
    
    const onDrop = (event) => {
      event.preventDefault()
      const nodeTypeData = event.dataTransfer.getData('text/plain')
      
      if (nodeTypeData) {
        const nodeType = JSON.parse(nodeTypeData)
        const rect = canvas.value.getBoundingClientRect()
        const newNode = {
          ...nodeType,
          id: Date.now().toString(),
          x: event.clientX - rect.left,
          y: event.clientY - rect.top
        }
        
        workflow.nodes.push(newNode)
      }
    }
    
    onMounted(() => {
      loadWorkflow()
    })
    
    return {
      workflow,
      selectedNode,
      showNodeLibrary,
      connectionDrag,
      executionStatus,
      canvas,
      saveWorkflow,
      runWorkflow,
      addNode,
      selectNode,
      updateNode,
      updateSelectedNode,
      deleteNode,
      deleteEdge,
      startConnection,
      onDrop
    }
  }
}
</script>

<style scoped>
.workflow-editor {
  height: 100vh;
  display: flex;
  flex-direction: column;
  background: #f5f5f5;
}

.editor-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background: white;
  border-bottom: 1px solid #ddd;
}

.editor-actions {
  display: flex;
  gap: 0.5rem;
}

.btn {
  padding: 0.5rem 1rem;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 500;
}

.btn-primary { background: #007bff; color: white; }
.btn-success { background: #28a745; color: white; }
.btn-secondary { background: #6c757d; color: white; }

.editor-content {
  flex: 1;
  display: flex;
  position: relative;
}

.node-library {
  width: 250px;
  background: white;
  border-right: 1px solid #ddd;
  padding: 1rem;
  overflow-y: auto;
}

.workflow-canvas {
  flex: 1;
  position: relative;
  overflow: hidden;
  background: #fafafa;
}

.connection-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  pointer-events: none;
  z-index: 100;
}

.properties-panel {
  width: 300px;
  background: white;
  border-left: 1px solid #ddd;
  padding: 1rem;
  overflow-y: auto;
}

.property-group {
  margin-bottom: 1rem;
}

.property-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
}

.property-group input,
.property-group textarea {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ddd;
  border-radius: 4px;
}

.execution-status {
  position: fixed;
  bottom: 1rem;
  right: 1rem;
  background: white;
  border: 1px solid #ddd;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
  max-width: 400px;
  z-index: 1000;
}

.status-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  border-bottom: 1px solid #ddd;
  background: #f8f9fa;
}

.status-content {
  padding: 1rem;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #6c757d;
}

.error {
  color: #dc3545;
  background: #f8d7da;
  padding: 0.5rem;
  border-radius: 4px;
}

pre {
  background: #f8f9fa;
  padding: 0.5rem;
  border-radius: 4px;
  overflow-x: auto;
  font-size: 0.875rem;
}
</style>
