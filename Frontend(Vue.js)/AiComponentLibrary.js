// AI Component Library - Defines all available AI and automation components

export default {
  name: 'AiComponentLibrary',
  template: `
    <div class="ai-component-library">
      <div v-for="category in categories" :key="category.name" class="component-category">
        <h4 class="category-title">{{ category.name }}</h4>
        <div
          v-for="component in category.components"
          :key="component.type"
          class="component-item"
          :draggable="true"
          @dragstart="startDrag($event, component)"
          @click="$emit('add-node', component)"
        >
          <div class="component-icon">
            <i :class="component.icon"></i>
          </div>
          <div class="component-info">
            <div class="component-name">{{ component.name }}</div>
            <div class="component-description">{{ component.description }}</div>
          </div>
        </div>
      </div>
    </div>
  `,
  emits: ['add-node'],
  data() {
    return {
      categories: [
        {
          name: 'AI Components',
          components: [
            {
              type: 'ai-text',
              name: 'Text Generation',
              description: 'Generate text using OpenAI GPT models',
              icon: 'fas fa-brain',
              inputs: [
                { id: 'prompt', name: 'Prompt', type: 'string' }
              ],
              outputs: [
                { id: 'text', name: 'Generated Text', type: 'string' }
              ],
              defaultProperties: {
                model: 'gpt-3.5-turbo',
                maxTokens: 150,
                temperature: 0.7
              }
            },
            {
              type: 'ai-image',
              name: 'Image Analysis',
              description: 'Analyze images using computer vision',
              icon: 'fas fa-image',
              inputs: [
                { id: 'image', name: 'Image', type: 'file' }
              ],
              outputs: [
                { id: 'analysis', name: 'Analysis Result', type: 'object' }
              ],
              defaultProperties: {
                provider: 'azure',
                features: ['description', 'tags', 'objects']
              }
            },
            {
              type: 'ai-speech',
              name: 'Speech to Text',
              description: 'Convert audio to text',
              icon: 'fas fa-microphone',
              inputs: [
                { id: 'audio', name: 'Audio File', type: 'file' }
              ],
              outputs: [
                { id: 'text', name: 'Transcribed Text', type: 'string' }
              ],
              defaultProperties: {
                language: 'en-US',
                provider: 'azure'
              }
            },
            {
              type: 'ai-translate',
              name: 'Language Translation',
              description: 'Translate text between languages',
              icon: 'fas fa-language',
              inputs: [
                { id: 'text', name: 'Input Text', type: 'string' }
              ],
              outputs: [
                { id: 'translated', name: 'Translated Text', type: 'string' }
              ],
              defaultProperties: {
                targetLanguage: 'es',
                provider: 'google'
              }
            },
            {
              type: 'ai-sentiment',
              name: 'Sentiment Analysis',
              description: 'Analyze text sentiment and emotions',
              icon: 'fas fa-heart',
              inputs: [
                { id: 'text', name: 'Input Text', type: 'string' }
              ],
              outputs: [
                { id: 'sentiment', name: 'Sentiment Score', type: 'object' }
              ],
              defaultProperties: {
                provider: 'azure',
                includeOpinions: true
              }
            }
          ]
        },
        {
          name: 'Automation',
          components: [
            {
              type: 'automation-email',
              name: 'Send Email',
              description: 'Send emails via SMTP',
              icon: 'fas fa-envelope',
              inputs: [
                { id: 'to', name: 'To Address', type: 'string' },
                { id: 'subject', name: 'Subject', type: 'string' },
                { id: 'body', name: 'Email Body', type: 'string' }
              ],
              outputs: [
                { id: 'status', name: 'Send Status', type: 'boolean' }
              ],
              defaultProperties: {
                smtpServer: 'smtp.gmail.com',
                port: 587,
                enableSsl: true
              }
            },
            {
              type: 'automation-slack',
              name: 'Slack Message',
              description: 'Send messages to Slack channels',
              icon: 'fab fa-slack',
              inputs: [
                { id: 'channel', name: 'Channel', type: 'string' },
                { id: 'message', name: 'Message', type: 'string' }
              ],
              outputs: [
                { id: 'messageId', name: 'Message ID', type: 'string' }
              ],
              defaultProperties: {
                botToken: '',
                username: 'Workflow Bot'
              }
            },
            {
              type: 'automation-zapier',
              name: 'Zapier Webhook',
              description: 'Trigger Zapier workflows',
              icon: 'fas fa-bolt',
              inputs: [
                { id: 'data', name: 'Webhook Data', type: 'object' }
              ],
              outputs: [
                { id: 'response', name: 'Webhook Response', type: 'object' }
              ],
              defaultProperties: {
                webhookUrl: '',
                method: 'POST'
              }
            },
            {
              type: 'automation-powerautomate',
              name: 'Power Automate',
              description: 'Trigger Microsoft Power Automate flows',
              icon: 'fas fa-cogs',
              inputs: [
                { id: 'trigger', name: 'Trigger Data', type: 'object' }
              ],
              outputs: [
                { id: 'result', name: 'Flow Result', type: 'object' }
              ],
              defaultProperties: {
                flowUrl: '',
                triggerName: ''
              }
            }
          ]
        },
        {
          name: 'Logic & Control',
          components: [
            {
              type: 'logic-condition',
              name: 'Condition',
              description: 'Conditional branching logic',
              icon: 'fas fa-code-branch',
              inputs: [
                { id: 'condition', name: 'Condition', type: 'boolean' },
                { id: 'trueValue', name: 'True Value', type: 'any' },
                { id: 'falseValue', name: 'False Value', type: 'any' }
              ],
              outputs: [
                { id: 'result', name: 'Result', type: 'any' }
              ],
              defaultProperties: {
                operator: 'equals',
                compareValue: ''
              }
            },
            {
              type: 'logic-loop',
              name: 'Loop',
              description: 'Iterate over data collections',
              icon: 'fas fa-sync-alt',
              inputs: [
                { id: 'collection', name: 'Collection', type: 'array' }
              ],
              outputs: [
                { id: 'item', name: 'Current Item', type: 'any' },
                { id: 'index', name: 'Index', type: 'number' }
              ],
              defaultProperties: {
                maxIterations: 100
              }
            },
            {
              type: 'logic-merge',
              name: 'Merge Data',
              description: 'Combine multiple data streams',
              icon: 'fas fa-compress-arrows-alt',
              inputs: [
                { id: 'input1', name: 'Input 1', type: 'any' },
                { id: 'input2', name: 'Input 2', type: 'any' }
              ],
              outputs: [
                { id: 'merged', name: 'Merged Result', type: 'object' }
              ],
              defaultProperties: {
                mergeStrategy: 'concat'
              }
            }
          ]
        },
        {
          name: 'Data & I/O',
          components: [
            {
              type: 'data-input',
              name: 'Manual Input',
              description: 'Manual data input point',
              icon: 'fas fa-play',
              inputs: [],
              outputs: [
                { id: 'value', name: 'Input Value', type: 'any' }
              ],
              defaultProperties: {
                inputType: 'text',
                defaultValue: ''
              }
            },
            {
              type: 'data-output',
              name: 'Output Display',
              description: 'Display workflow results',
              icon: 'fas fa-flag-checkered',
              inputs: [
                { id: 'data', name: 'Data to Display', type: 'any' }
              ],
              outputs: [],
              defaultProperties: {
                format: 'json',
                showTimestamp: true
              }
            },
            {
              type: 'data-transform',
              name: 'Transform Data',
              description: 'Transform and manipulate data',
              icon: 'fas fa-exchange-alt',
              inputs: [
                { id: 'input', name: 'Input Data', type: 'any' }
              ],
              outputs: [
                { id: 'output', name: 'Transformed Data', type: 'any' }
              ],
              defaultProperties: {
                transformation: 'identity',
                jsonPath: '$'
              }
            }
          ]
        }
      ]
    }
  },
  methods: {
    startDrag(event, component) {
      event.dataTransfer.setData('text/plain', JSON.stringify(component))
    }
  }
}

// Export component definitions for use in other parts of the application
export const AI_COMPONENTS = {
  TEXT_GENERATION: 'ai-text',
  IMAGE_ANALYSIS: 'ai-image',
  SPEECH_TO_TEXT: 'ai-speech',
  TRANSLATION: 'ai-translate',
  SENTIMENT_ANALYSIS: 'ai-sentiment'
}

export const AUTOMATION_COMPONENTS = {
  EMAIL: 'automation-email',
  SLACK: 'automation-slack',
  ZAPIER: 'automation-zapier',
  POWER_AUTOMATE: 'automation-powerautomate'
}

export const LOGIC_COMPONENTS = {
  CONDITION: 'logic-condition',
  LOOP: 'logic-loop',
  MERGE: 'logic-merge'
}

export const DATA_COMPONENTS = {
  INPUT: 'data-input',
  OUTPUT: 'data-output',
  TRANSFORM: 'data-transform'
}

// Helper function to get component definition by type
export function getComponentDefinition(componentType) {
  const categories = [
    {
      name: 'AI Components',
      components: [
        // ... (same as above, could be extracted to a shared constant)
      ]
    }
    // ... other categories
  ]
  
  for (const category of categories) {
    const component = category.components.find(c => c.type === componentType)
    if (component) return component
  }
  
  return null
}

// Validation functions for different component types
export const ComponentValidators = {
  'ai-text': (properties) => {
    const errors = []
    if (!properties.model) errors.push('Model is required')
    if (properties.maxTokens < 1 || properties.maxTokens > 4096) {
      errors.push('Max tokens must be between 1 and 4096')
    }
    return errors
  },
  
  'ai-image': (properties) => {
    const errors = []
    if (!properties.provider) errors.push('Provider is required')
    if (!properties.features || properties.features.length === 0) {
      errors.push('At least one analysis feature must be selected')
    }
    return errors
  },
  
  'automation-email': (properties) => {
    const errors = []
    if (!properties.smtpServer) errors.push('SMTP server is required')
    if (!properties.port || properties.port < 1 || properties.port > 65535) {
      errors.push('Valid port number is required')
    }
    return errors
  }
  
  // Add more validators as needed
}