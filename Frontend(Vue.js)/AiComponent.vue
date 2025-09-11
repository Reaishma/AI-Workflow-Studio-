<template>
  <div class="ai-component-config">
    <!-- Text Generation Component -->
    <div v-if="node.type === 'ai-text'" class="config-section">
      <h4>Text Generation Settings</h4>
      
      <div class="config-field">
        <label>AI Model:</label>
        <select v-model="properties.model" @change="updateProperties">
          <option value="gpt-3.5-turbo">GPT-3.5 Turbo</option>
          <option value="gpt-4">GPT-4</option>
          <option value="gpt-4-turbo">GPT-4 Turbo</option>
          <option value="claude-3-sonnet">Claude 3 Sonnet</option>
          <option value="claude-3-haiku">Claude 3 Haiku</option>
        </select>
      </div>
      
      <div class="config-field">
        <label>Max Tokens:</label>
        <input 
          type="number" 
          v-model.number="properties.maxTokens" 
          min="1" 
          max="4096" 
          @input="updateProperties"
        />
      </div>
      
      <div class="config-field">
        <label>Temperature:</label>
        <input 
          type="range" 
          v-model.number="properties.temperature" 
          min="0" 
          max="2" 
          step="0.1" 
          @input="updateProperties"
        />
        <span class="range-value">{{ properties.temperature }}</span>
      </div>
      
      <div class="config-field">
        <label>System Prompt:</label>
        <textarea 
          v-model="properties.systemPrompt" 
          placeholder="Optional system prompt to guide the AI's behavior..."
          @input="updateProperties"
        ></textarea>
      </div>
    </div>
    
    <!-- Image Analysis Component -->
    <div v-if="node.type === 'ai-image'" class="config-section">
      <h4>Image Analysis Settings</h4>
      
      <div class="config-field">
        <label>AI Provider:</label>
        <select v-model="properties.provider" @change="updateProperties">
          <option value="azure">Azure Computer Vision</option>
          <option value="google">Google Cloud Vision</option>
          <option value="openai">OpenAI Vision</option>
          <option value="anthropic">Anthropic Claude Vision</option>
        </select>
      </div>
      
      <div class="config-field">
        <label>Analysis Features:</label>
        <div class="checkbox-group">
          <label class="checkbox-item">
            <input 
              type="checkbox" 
              value="description" 
              v-model="properties.features" 
              @change="updateProperties"
            />
            Description
          </label>
          <label class="checkbox-item">
            <input 
              type="checkbox" 
              value="tags" 
              v-model="properties.features" 
              @change="updateProperties"
            />
            Tags
          </label>
          <label class="checkbox-item">
            <input 
              type="checkbox" 
              value="objects" 
              v-model="properties.features" 
              @change="updateProperties"
            />
            Object Detection
          </label>
          <label class="checkbox-item">
            <input 
              type="checkbox" 
              value="faces" 
              v-model="properties.features" 
              @change="updateProperties"
            />
            Face Detection
          </label>
          <label class="checkbox-item">
            <input 
              type="checkbox" 
              value="text" 
              v-model="properties.features" 
              @change="updateProperties"
            />
            Text Recognition (OCR)
          </label>
        </div>
      </div>
      
      <div class="config-field">
        <label>Custom Prompt:</label>
        <textarea 
          v-model="properties.customPrompt" 
          placeholder="Describe what you want to know about the image..."
          @input="updateProperties"
        ></textarea>
      </div>
    </div>
    
    <!-- Speech to Text Component -->
    <div v-if="node.type === 'ai-speech'" class="config-section">
      <h4>Speech Recognition Settings</h4>
      
      <div class="config-field">
        <label>Provider:</label>
        <select v-model="properties.provider" @change="updateProperties">
          <option value="azure">Azure Speech Services</option>
          <option value="google">Google Speech-to-Text</option>
          <option value="openai">OpenAI Whisper</option>
          <option value="aws">AWS Transcribe</option>
        </select>
      </div>
      
      <div class="config-field">
        <label>Language:</label>
        <select v-model="properties.language" @change="updateProperties">
          <option value="en-US">English (US)</option>
          <option value="en-GB">English (UK)</option>
          <option value="es-ES">Spanish</option>
          <option value="fr-FR">French</option>
          <option value="de-DE">German</option>
          <option value="it-IT">Italian</option>
          <option value="pt-BR">Portuguese (Brazil)</option>
          <option value="zh-CN">Chinese (Mandarin)</option>
          <option value="ja-JP">Japanese</option>
          <option value="ko-KR">Korean</option>
        </select>
      </div>
      
      <div class="config-field">
        <label class="checkbox-item">
          <input 
            type="checkbox" 
            v-model="properties.enablePunctuation" 
            @change="updateProperties"
          />
          Enable Automatic Punctuation
        </label>
      </div>
      
      <div class="config-field">
        <label class="checkbox-item">
          <input 
            type="checkbox" 
            v-model="properties.enableTimestamps" 
            @change="updateProperties"
          />
          Include Word Timestamps
        </label>
      </div>
    </div>
    
    <!-- Translation Component -->
    <div v-if="node.type === 'ai-translate'" class="config-section">
      <h4>Translation Settings</h4>
      
      <div class="config-field">
        <label>Provider:</label>
        <select v-model="properties.provider" @change="updateProperties">
          <option value="google">Google Translate</option>
          <option value="azure">Azure Translator</option>
          <option value="aws">AWS Translate</option>
          <option value="deepl">DeepL</option>
        </select>
      </div>
      
      <div class="config-field">
        <label>Source Language:</label>
        <select v-model="properties.sourceLanguage" @change="updateProperties">
          <option value="auto">Auto-detect</option>
          <option value="en">English</option>
          <option value="es">Spanish</option>
          <option value="fr">French</option>
          <option value="de">German</option>
          <option value="it">Italian</option>
          <option value="pt">Portuguese</option>
          <option value="zh">Chinese</option>
          <option value="ja">Japanese</option>
          <option value="ko">Korean</option>
        </select>
      </div>
      
      <div class="config-field">
        <label>Target Language:</label>
        <select v-model="properties.targetLanguage" @change="updateProperties">
          <option value="en">English</option>
          <option value="es">Spanish</option>
          <option value="fr">French</option>
          <option value="de">German</option>
          <option value="it">Italian</option>
          <option value="pt">Portuguese</option>
          <option value="zh">Chinese</option>
          <option value="ja">Japanese</option>
          <option value="ko">Korean</option>
        </select>
      </div>
      
      <div class="config-field">
        <label class="checkbox-item">
          <input 
            type="checkbox" 
            v-model="properties.preserveFormatting" 
            @change="updateProperties"
          />
          Preserve Text Formatting
        </label>
      </div>
    </div>
    
    <!-- Sentiment Analysis Component -->
    <div v-if="node.type === 'ai-sentiment'" class="config-section">
      <h4>Sentiment Analysis Settings</h4>
      
      <div class="config-field">
        <label>Provider:</label>
        <select v-model="properties.provider" @change="updateProperties">
          <option value="azure">Azure Text Analytics</option>
          <option value="google">Google Natural Language</option>
          <option value="aws">AWS Comprehend</option>
          <option value="openai">OpenAI</option>
        </select>
      </div>
      
      <div class="config-field">
        <label>Analysis Depth:</label>
        <select v-model="properties.analysisDepth" @change="updateProperties">
          <option value="basic">Basic Sentiment (Positive/Negative/Neutral)</option>
          <option value="detailed">Detailed Emotions</option>
          <option value="aspectBased">Aspect-based Sentiment</option>
        </select>
      </div>
      
      <div class="config-field">
        <label class="checkbox-item">
          <input 
            type="checkbox" 
            v-model="properties.includeConfidence" 
            @change="updateProperties"
          />
          Include Confidence Scores
        </label>
      </div>
      
      <div class="config-field">
        <label class="checkbox-item">
          <input 
            type="checkbox" 
            v-model="properties.detectLanguage" 
            @change="updateProperties"
          />
          Auto-detect Language
        </label>
      </div>
    </div>
    
    <!-- Automation Components -->
    <div v-if="node.type.startsWith('automation')" class="config-section">
      <h4>{{ getAutomationTitle(node.type) }} Settings</h4>
      
      <!-- Email Configuration -->
      <div v-if="node.type === 'automation-email'">
        <div class="config-field">
          <label>SMTP Server:</label>
          <input 
            type="text" 
            v-model="properties.smtpServer" 
            placeholder="smtp.gmail.com"
            @input="updateProperties"
          />
        </div>
        
        <div class="config-field">
          <label>Port:</label>
          <input 
            type="number" 
            v-model.number="properties.port" 
            placeholder="587"
            @input="updateProperties"
          />
        </div>
        
        <div class="config-field">
          <label class="checkbox-item">
            <input 
              type="checkbox" 
              v-model="properties.enableSsl" 
              @change="updateProperties"
            />
            Enable SSL/TLS
          </label>
        </div>
        
        <div class="config-field">
          <label>From Name:</label>
          <input 
            type="text" 
            v-model="properties.fromName" 
            placeholder="Workflow System"
            @input="updateProperties"
          />
        </div>
      </div>
      
      <!-- Slack Configuration -->
      <div v-if="node.type === 'automation-slack'">
        <div class="config-field">
          <label>Bot Token:</label>
          <input 
            type="password" 
            v-model="properties.botToken" 
            placeholder="xoxb-..."
            @input="updateProperties"
          />
        </div>
        
        <div class="config-field">
          <label>Default Channel:</label>
          <input 
            type="text" 
            v-model="properties.defaultChannel" 
            placeholder="#general"
            @input="updateProperties"
          />
        </div>
        
        <div class="config-field">
          <label>Bot Username:</label>
          <input 
            type="text" 
            v-model="properties.username" 
            placeholder="Workflow Bot"
            @input="updateProperties"
          />
        </div>
      </div>
    </div>
    
    <!-- Validation Errors -->
    <div v-if="validationErrors.length > 0" class="validation-errors">
      <h5>Configuration Issues:</h5>
      <ul>
        <li v-for="error in validationErrors" :key="error" class="error-item">
          {{ error }}
        </li>
      </ul>
    </div>
  </div>
</template>

<script>
import { ref, computed, watch } from 'vue'

export default {
  name: 'AiComponent',
  props: {
    node: {
      type: Object,
      required: true
    }
  },
  emits: ['update'],
  setup(props, { emit }) {
    const properties = ref({ ...props.node.properties })
    
    const validationErrors = computed(() => {
      const errors = []
      
      // Validate based on component type
      switch (props.node.type) {
        case 'ai-text':
          if (!properties.value.model) errors.push('AI model is required')
          if (properties.value.maxTokens < 1 || properties.value.maxTokens > 4096) {
            errors.push('Max tokens must be between 1 and 4096')
          }
          if (properties.value.temperature < 0 || properties.value.temperature > 2) {
            errors.push('Temperature must be between 0 and 2')
          }
          break
          
        case 'ai-image':
          if (!properties.value.provider) errors.push('AI provider is required')
          if (!properties.value.features || properties.value.features.length === 0) {
            errors.push('At least one analysis feature must be selected')
          }
          break
          
        case 'ai-speech':
          if (!properties.value.provider) errors.push('Speech provider is required')
          if (!properties.value.language) errors.push('Language is required')
          break
          
        case 'ai-translate':
          if (!properties.value.provider) errors.push('Translation provider is required')
          if (!properties.value.targetLanguage) errors.push('Target language is required')
          break
          
        case 'ai-sentiment':
          if (!properties.value.provider) errors.push('Sentiment provider is required')
          break
          
        case 'automation-email':
          if (!properties.value.smtpServer) errors.push('SMTP server is required')
          if (!properties.value.port || properties.value.port < 1 || properties.value.port > 65535) {
            errors.push('Valid port number is required')
          }
          break
          
        case 'automation-slack':
          if (!properties.value.botToken) errors.push('Slack bot token is required')
          break
      }
      
      return errors
    })
    
    const getAutomationTitle = (type) => {
      const titles = {
        'automation-email': 'Email',
        'automation-slack': 'Slack',
        'automation-zapier': 'Zapier',
        'automation-powerautomate': 'Power Automate'
      }
      return titles[type] || 'Automation'
    }
    
    const updateProperties = () => {
      const updatedNode = {
        ...props.node,
        properties: { ...properties.value }
      }
      emit('update', updatedNode)
    }
    
    // Watch for external changes to node properties
    watch(() => props.node.properties, (newProperties) => {
      properties.value = { ...newProperties }
    }, { deep: true })
    
    return {
      properties,
      validationErrors,
      getAutomationTitle,
      updateProperties
    }
  }
}
</script>

<style scoped>
.ai-component-config {
  padding: 1rem;
}

.config-section {
  margin-bottom: 1.5rem;
}

.config-section h4 {
  margin: 0 0 1rem 0;
  color: #495057;
  font-size: 1rem;
  font-weight: 600;
}

.config-field {
  margin-bottom: 1rem;
}

.config-field label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: #495057;
  font-size: 0.9rem;
}

.config-field input,
.config-field select,
.config-field textarea {
  width: 100%;
  padding: 0.5rem;
  border: 1px solid #ced4da;
  border-radius: 4px;
  font-size: 0.9rem;
  transition: border-color 0.2s ease;
}

.config-field input:focus,
.config-field select:focus,
.config-field textarea:focus {
  outline: none;
  border-color: #007bff;
  box-shadow: 0 0 0 2px rgba(0,123,255,0.25);
}

.config-field textarea {
  resize: vertical;
  min-height: 60px;
}

.config-field input[type="range"] {
  width: calc(100% - 40px);
  margin-right: 10px;
}

.range-value {
  font-weight: 500;
  color: #495057;
  font-size: 0.9rem;
}

.checkbox-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.checkbox-item {
  display: flex;
  align-items: center;
  font-weight: normal !important;
  cursor: pointer;
}

.checkbox-item input[type="checkbox"] {
  width: auto;
  margin-right: 0.5rem;
  margin-bottom: 0;
}

.validation-errors {
  background: #f8d7da;
  border: 1px solid #f5c6cb;
  border-radius: 4px;
  padding: 1rem;
  margin-top: 1rem;
}

.validation-errors h5 {
  margin: 0 0 0.5rem 0;
  color: #721c24;
  font-size: 0.9rem;
}

.validation-errors ul {
  margin: 0;
  padding-left: 1.5rem;
}

.error-item {
  color: #721c24;
  font-size: 0.85rem;
  margin-bottom: 0.25rem;
}

/* Component-specific styling */
.ai-component-config .config-section {
  border-left: 3px solid #28a745;
  padding-left: 1rem;
}

.automation-email .config-section {
  border-left-color: #17a2b8;
}

.automation-slack .config-section {
  border-left-color: #4a154b;
}

.automation-zapier .config-section {
  border-left-color: #ff4a00;
}
</style>