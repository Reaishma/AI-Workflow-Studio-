<template>
  <div
    class="workflow-node"
    :class="{ selected, 'ai-node': node.type.startsWith('ai') }"
    :style="{ left: node.x + 'px', top: node.y + 'px' }"
    @click="$emit('select', node)"
    @mousedown="startDrag"
  >
    <div class="node-header">
      <div class="node-icon">
        <i :class="getNodeIcon(node.type)"></i>
      </div>
      <div class="node-title">{{ node.name }}</div>
      <button class="node-delete" @click.stop="$emit('delete', node.id)">&times;</button>
    </div>
    
    <div class="node-body">
      <div class="node-description">{{ node.description }}</div>
      
      <!-- Input Ports -->
      <div v-if="node.inputs?.length" class="node-ports inputs">
        <div
          v-for="input in node.inputs"
          :key="input.id"
          class="node-port input"
          :data-node-id="node.id"
          :data-port-id="input.id"
          :title="input.name"
        >
          <div class="port-dot"></div>
          <span class="port-label">{{ input.name }}</span>
        </div>
      </div>
      
      <!-- Output Ports -->
      <div v-if="node.outputs?.length" class="node-ports outputs">
        <div
          v-for="output in node.outputs"
          :key="output.id"
          class="node-port output"
          :data-node-id="node.id"
          :data-port-id="output.id"
          :title="output.name"
          @mousedown.stop="startConnection($event, output)"
        >
          <span class="port-label">{{ output.name }}</span>
          <div class="port-dot"></div>
        </div>
      </div>
    </div>
    
    <!-- Node Status Indicator -->
    <div v-if="node.status" class="node-status" :class="node.status">
      <i :class="getStatusIcon(node.status)"></i>
    </div>
  </div>
</template>

<script>
import { ref } from 'vue'

export default {
  name: 'WorkflowNode',
  props: {
    node: {
      type: Object,
      required: true
    },
    selected: {
      type: Boolean,
      default: false
    }
  },
  emits: ['select', 'update', 'delete', 'connect'],
  setup(props, { emit }) {
    const dragState = ref(null)
    
    const getNodeIcon = (nodeType) => {
      const iconMap = {
        'ai-text': 'fas fa-brain',
        'ai-image': 'fas fa-image',
        'ai-speech': 'fas fa-microphone',
        'ai-translate': 'fas fa-language',
        'ai-sentiment': 'fas fa-heart',
        'automation-email': 'fas fa-envelope',
        'automation-slack': 'fab fa-slack',
        'automation-zapier': 'fas fa-bolt',
        'logic-condition': 'fas fa-code-branch',
        'logic-loop': 'fas fa-sync-alt',
        'data-input': 'fas fa-play',
        'data-output': 'fas fa-flag-checkered',
        'data-transform': 'fas fa-exchange-alt'
      }
      return iconMap[nodeType] || 'fas fa-cube'
    }
    
    const getStatusIcon = (status) => {
      const statusIcons = {
        'running': 'fas fa-spinner fa-spin',
        'completed': 'fas fa-check-circle',
        'failed': 'fas fa-exclamation-circle',
        'pending': 'fas fa-clock'
      }
      return statusIcons[status] || 'fas fa-circle'
    }
    
    const startDrag = (event) => {
      event.preventDefault()
      
      dragState.value = {
        startX: event.clientX - props.node.x,
        startY: event.clientY - props.node.y
      }
      
      document.addEventListener('mousemove', onDrag)
      document.addEventListener('mouseup', endDrag)
    }
    
    const onDrag = (event) => {
      if (dragState.value) {
        const newX = event.clientX - dragState.value.startX
        const newY = event.clientY - dragState.value.startY
        
        const updatedNode = {
          ...props.node,
          x: Math.max(0, newX),
          y: Math.max(0, newY)
        }
        
        emit('update', updatedNode)
      }
    }
    
    const endDrag = () => {
      dragState.value = null
      document.removeEventListener('mousemove', onDrag)
      document.removeEventListener('mouseup', endDrag)
    }
    
    const startConnection = (event, output) => {
      event.preventDefault()
      event.stopPropagation()
      
      const rect = event.currentTarget.getBoundingClientRect()
      const parentRect = event.currentTarget.closest('.workflow-canvas').getBoundingClientRect()
      
      emit('connect', {
        nodeId: props.node.id,
        portId: output.id,
        x: rect.left + rect.width / 2 - parentRect.left,
        y: rect.top + rect.height / 2 - parentRect.top
      })
    }
    
    return {
      getNodeIcon,
      getStatusIcon,
      startDrag,
      startConnection
    }
  }
}
</script>

<style scoped>
.workflow-node {
  position: absolute;
  background: white;
  border: 2px solid #ddd;
  border-radius: 8px;
  min-width: 180px;
  cursor: move;
  user-select: none;
  box-shadow: 0 2px 8px rgba(0,0,0,0.1);
  transition: all 0.2s ease;
}

.workflow-node:hover {
  border-color: #007bff;
  box-shadow: 0 4px 12px rgba(0,123,255,0.15);
}

.workflow-node.selected {
  border-color: #007bff;
  box-shadow: 0 0 0 2px rgba(0,123,255,0.25);
}

.workflow-node.ai-node {
  border-color: #28a745;
}

.workflow-node.ai-node:hover,
.workflow-node.ai-node.selected {
  border-color: #28a745;
  box-shadow: 0 0 0 2px rgba(40,167,69,0.25);
}

.node-header {
  display: flex;
  align-items: center;
  padding: 0.75rem;
  background: #f8f9fa;
  border-bottom: 1px solid #ddd;
  border-radius: 6px 6px 0 0;
}

.node-icon {
  margin-right: 0.5rem;
  color: #6c757d;
  font-size: 1.1rem;
}

.ai-node .node-icon {
  color: #28a745;
}

.node-title {
  flex: 1;
  font-weight: 600;
  font-size: 0.9rem;
}

.node-delete {
  background: none;
  border: none;
  font-size: 1.2rem;
  color: #dc3545;
  cursor: pointer;
  padding: 0;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.node-delete:hover {
  background: rgba(220,53,69,0.1);
  border-radius: 50%;
}

.node-body {
  padding: 0.75rem;
}

.node-description {
  font-size: 0.8rem;
  color: #6c757d;
  margin-bottom: 0.5rem;
  line-height: 1.3;
}

.node-ports {
  margin: 0.5rem 0;
}

.node-port {
  display: flex;
  align-items: center;
  margin: 0.25rem 0;
  font-size: 0.8rem;
}

.node-port.input {
  justify-content: flex-start;
}

.node-port.output {
  justify-content: flex-end;
  cursor: pointer;
}

.port-dot {
  width: 8px;
  height: 8px;
  border-radius: 50%;
  background: #6c757d;
  border: 2px solid white;
  box-shadow: 0 0 0 1px #6c757d;
  transition: all 0.2s ease;
}

.node-port.input .port-dot {
  margin-right: 0.5rem;
}

.node-port.output .port-dot {
  margin-left: 0.5rem;
}

.node-port.output:hover .port-dot {
  background: #007bff;
  box-shadow: 0 0 0 1px #007bff;
  transform: scale(1.2);
}

.port-label {
  color: #495057;
  font-weight: 500;
}

.node-status {
  position: absolute;
  top: -8px;
  right: -8px;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 0.7rem;
  border: 2px solid white;
}

.node-status.running {
  background: #ffc107;
  color: white;
}

.node-status.completed {
  background: #28a745;
  color: white;
}

.node-status.failed {
  background: #dc3545;
  color: white;
}

.node-status.pending {
  background: #6c757d;
  color: white;
}

@keyframes pulse {
  0% { transform: scale(1); }
  50% { transform: scale(1.1); }
  100% { transform: scale(1); }
}

.node-status.running {
  animation: pulse 1.5s infinite;
}
</style>