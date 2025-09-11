<template>
  <svg class="workflow-edge" :style="svgStyle">
    <defs>
      <marker
        id="arrowhead"
        markerWidth="10"
        markerHeight="7"
        refX="9"
        refY="3.5"
        orient="auto"
      >
        <polygon points="0 0, 10 3.5, 0 7" fill="#6c757d" />
      </marker>
    </defs>
    
    <path
      :d="pathData"
      :stroke="edgeColor"
      stroke-width="2"
      fill="none"
      marker-end="url(#arrowhead)"
      class="edge-path"
      @click="selectEdge"
    />
    
    <!-- Delete button for selected edge -->
    <circle
      v-if="selected"
      :cx="midPoint.x"
      :cy="midPoint.y"
      r="10"
      fill="#dc3545"
      class="delete-button"
      @click="$emit('delete', edge.id)"
    />
    <text
      v-if="selected"
      :x="midPoint.x"
      :y="midPoint.y + 1"
      text-anchor="middle"
      fill="white"
      font-size="12"
      font-weight="bold"
      class="delete-text"
      @click="$emit('delete', edge.id)"
    >
      ×
    </text>
  </svg>
</template>

<script>
import { computed, ref } from 'vue'

export default {
  name: 'WorkflowEdge',
  props: {
    edge: {
      type: Object,
      required: true
    },
    selected: {
      type: Boolean,
      default: false
    }
  },
  emits: ['delete', 'select'],
  setup(props, { emit }) {
    const sourcePoint = ref({ x: 0, y: 0 })
    const targetPoint = ref({ x: 0, y: 0 })
    
    // Calculate connection points based on node positions
    const updateConnectionPoints = () => {
      const sourceNode = document.querySelector(`[data-node-id="${props.edge.sourceNodeId}"]`)
      const targetNode = document.querySelector(`[data-node-id="${props.edge.targetNodeId}"]`)
      const sourcePort = document.querySelector(`[data-node-id="${props.edge.sourceNodeId}"][data-port-id="${props.edge.sourcePortId}"]`)
      const targetPort = document.querySelector(`[data-node-id="${props.edge.targetNodeId}"][data-port-id="${props.edge.targetPortId}"]`)
      
      if (sourcePort && targetPort) {
        const canvas = document.querySelector('.workflow-canvas')
        const canvasRect = canvas.getBoundingClientRect()
        
        const sourceRect = sourcePort.getBoundingClientRect()
        const targetRect = targetPort.getBoundingClientRect()
        
        sourcePoint.value = {
          x: sourceRect.left + sourceRect.width / 2 - canvasRect.left,
          y: sourceRect.top + sourceRect.height / 2 - canvasRect.top
        }
        
        targetPoint.value = {
          x: targetRect.left + targetRect.width / 2 - canvasRect.left,
          y: targetRect.top + targetRect.height / 2 - canvasRect.top
        }
      }
    }
    
    // Update connection points on mount and when edge changes
    setTimeout(updateConnectionPoints, 100)
    
    const svgStyle = computed(() => {
      const minX = Math.min(sourcePoint.value.x, targetPoint.value.x) - 20
      const minY = Math.min(sourcePoint.value.y, targetPoint.value.y) - 20
      const maxX = Math.max(sourcePoint.value.x, targetPoint.value.x) + 20
      const maxY = Math.max(sourcePoint.value.y, targetPoint.value.y) + 20
      
      return {
        position: 'absolute',
        left: minX + 'px',
        top: minY + 'px',
        width: (maxX - minX) + 'px',
        height: (maxY - minY) + 'px',
        pointerEvents: 'none',
        zIndex: 1
      }
    })
    
    const pathData = computed(() => {
      const start = {
        x: sourcePoint.value.x - (Math.min(sourcePoint.value.x, targetPoint.value.x) - 20),
        y: sourcePoint.value.y - (Math.min(sourcePoint.value.y, targetPoint.value.y) - 20)
      }
      
      const end = {
        x: targetPoint.value.x - (Math.min(sourcePoint.value.x, targetPoint.value.x) - 20),
        y: targetPoint.value.y - (Math.min(sourcePoint.value.y, targetPoint.value.y) - 20)
      }
      
      // Create a curved path (Bezier curve)
      const controlOffset = Math.abs(end.x - start.x) * 0.5
      const control1 = { x: start.x + controlOffset, y: start.y }
      const control2 = { x: end.x - controlOffset, y: end.y }
      
      return `M ${start.x} ${start.y} C ${control1.x} ${control1.y}, ${control2.x} ${control2.y}, ${end.x} ${end.y}`
    })
    
    const midPoint = computed(() => {
      return {
        x: (sourcePoint.value.x + targetPoint.value.x) / 2 - (Math.min(sourcePoint.value.x, targetPoint.value.x) - 20),
        y: (sourcePoint.value.y + targetPoint.value.y) / 2 - (Math.min(sourcePoint.value.y, targetPoint.value.y) - 20)
      }
    })
    
    const edgeColor = computed(() => {
      if (props.selected) return '#dc3545'
      if (props.edge.animated) return '#28a745'
      return '#6c757d'
    })
    
    const selectEdge = (event) => {
      event.stopPropagation()
      emit('select', props.edge)
    }
    
    // Watch for node position changes and update connection points
    const observer = new MutationObserver(updateConnectionPoints)
    observer.observe(document.body, { 
      childList: true, 
      subtree: true, 
      attributes: true, 
      attributeFilter: ['style'] 
    })
    
    return {
      svgStyle,
      pathData,
      midPoint,
      edgeColor,
      selectEdge
    }
  }
}
</script>

<style scoped>
.workflow-edge {
  overflow: visible;
}

.edge-path {
  cursor: pointer;
  stroke-dasharray: 0;
  transition: all 0.2s ease;
  pointer-events: stroke;
  stroke-width: 8px;
  stroke: transparent;
}

.edge-path:hover {
  stroke: rgba(0, 123, 255, 0.3) !important;
}

.delete-button,
.delete-text {
  cursor: pointer;
  pointer-events: all;
}

.delete-button:hover {
  fill: #c82333;
}

/* Animation for active data flow */
@keyframes flow {
  0% {
    stroke-dashoffset: 10;
  }
  100% {
    stroke-dashoffset: 0;
  }
}

.edge-path.animated {
  stroke-dasharray: 5, 5;
  animation: flow 1s linear infinite;
}
</style>