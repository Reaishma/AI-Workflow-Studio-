# Visual AI Workflow Designer

![Visual AI Workflow Designer](https://img.shields.io/badge/Status-Complete-brightgreen) ![Technology](https://img.shields.io/badge/Tech-Vue.js%2BCSS3%2B.NETCore-blue) ![License](https://img.shields.io/badge/License-MIT-green)

A comprehensive low-code visual AI workflow editor that allows users to create AI-powered workflows without coding. The platform integrates Microsoft Azure cognitive services, Google Cloud AI platform, and automation tools like Zapier and Microsoft Power Automate.

## 🎯 Project Overview

**Build powerful AI workflows visually** - No coding required! This platform provides a drag-and-drop interface for creating sophisticated AI automation workflows that can process text, images, audio, and integrate with popular business tools.

### 🎨 Key Features

#### **Visual Workflow Designer**
- **Drag-and-Drop Interface** - Intuitive visual editor for building workflows
- **Real-time Connections** - Connect components with visual connectors
- **Interactive Canvas** - Pan, zoom, and organize your workflow visually
- **Live Execution** - Watch your workflow execute in real-time

#### **🤖 AI Components Library**
- **Text Generation** - GPT-3.5, GPT-4, Claude 3 integration
- **Image Analysis** - Computer vision, object detection, OCR
- **Speech-to-Text** - Audio transcription and voice processing
- **Language Translation** - Multi-language support with Google/Azure APIs
- **Sentiment Analysis** - Emotional tone detection and analysis

#### **⚡ Automation Components**
- **Email Integration** - SMTP configuration and automated sending
- **Slack Messaging** - Bot integration and team notifications
- **Zapier Webhooks** - Trigger external automation workflows
- **Power Automate** - Microsoft flow integration

#### **🔧 Logic & Control Components**
- **Conditional Logic** - If/then decision making
- **Loops & Iteration** - Process collections of data
- **Data Input/Output** - Manual inputs and result display
- **Variable Management** - Store and manipulate workflow data

### 🏗️ Architecture

#### **Frontend Technologies**
- **HTML5/CSS3** - Modern responsive design
- **Vanilla JavaScript** - No framework dependencies
- **Font Awesome** - Professional iconography
- **SVG Graphics** - Scalable connection lines and diagrams

#### **Backend Integration** (.NET Core API)
- **RESTful APIs** - Workflow management and execution
- **Entity Framework** - Data persistence and relationships
- **JWT Authentication** - Secure user management
- **Azure/Google Cloud** - AI service integrations

#### **Database Schema** (PostgreSQL)
```sql
-- Core workflow storage
Workflows: id, name, description, definition, version, user_id
WorkflowExecutions: id, workflow_id, status, start_time, end_time
Users: id, username, email, created_at
```

## 🚀 Getting Started

### Option 1: Standalone HTML File
**Perfect for demos, prototyping, and simple deployments**

1. **Download** the `visual-ai-workflow-designer.html` file
2. **Open** in any modern web browser
3. **Start building** workflows immediately!

```bash
# Simply open the file in your browser
open visual-ai-workflow-designer.html
# or
firefox visual-ai-workflow-designer.html
```

### Option 2: Full Application Setup
**Complete frontend/backend architecture**

#### Prerequisites
- Node.js 18+ and npm
- .NET 6.0+ SDK
- PostgreSQL database

#### Backend Setup (.NET API)
```bash
# Navigate to the API project
cd services/workflow/WorkflowApi

# Restore dependencies
dotnet restore

# Update database connection string in appsettings.json
# Run database migrations
dotnet ef database update

# Start the API server
dotnet run
# API will be available at https://localhost:5001
```

#### Frontend Setup (Vue.js)
```bash
# Install dependencies
npm install

# Configure API endpoint in vue app
# Update .env.local with your API URL

# Start development server
npm run dev
# Frontend will be available at http://localhost:5000
```

## 📖 Usage Guide

### Building Your First Workflow

1. **Start with Input** - Drag an "Input" component to define your starting data
2. **Add AI Processing** - Add AI components like "Text Generation" or "Image Analysis"
3. **Configure Properties** - Select components and set their properties in the right panel
4. **Connect Components** - Click output ports and drag to input ports to connect
5. **Add Logic** - Use conditions and loops for complex decision making
6. **Finish with Output** - End with automation or output components
7. **Save & Execute** - Save your workflow and run it to see results

### Example Workflows

#### 🎧 **Customer Support Automation**
```
Customer Message → Sentiment Analysis → Condition → AI Response → Email/Slack
```

#### 📝 **Content Moderation**
```
User Content → AI Text Analysis → Safety Check → Alert Moderators
```

#### 📄 **Document Processing**
```
Document Upload → OCR Extraction → Information Extraction → Results
```

#### 📱 **Social Media Monitoring**
```
Social Trigger → Sentiment Analysis → Filter Important → Team Notification
```

### Component Configuration

#### **AI Text Generation**
- **Model Selection**: GPT-3.5, GPT-4, Claude 3 Sonnet
- **Parameters**: Max tokens (1-4096), Temperature (0.0-2.0)
- **System Prompt**: Custom instructions for the AI

#### **Image Analysis**
- **Providers**: Azure Computer Vision, Google Cloud Vision, OpenAI Vision
- **Features**: Description, Tags, Object Detection, OCR
- **Custom Prompts**: Specific analysis instructions

#### **Email Automation**
- **SMTP Configuration**: Server, port, SSL/TLS settings
- **Dynamic Content**: Use workflow data in email content
- **Attachments**: Include files from previous steps

## 🔧 Advanced Features

### **Workflow Templates**
Pre-built workflows for common use cases:
- AI Customer Support with sentiment routing
- Content moderation with safety checks
- Document processing with OCR
- Social media monitoring with alerts

### **Keyboard Shortcuts**
- `Ctrl/Cmd + S` - Save workflow
- `Ctrl/Cmd + N` - New workflow
- `Delete` - Delete selected node
- `Ctrl/Cmd + Z` - Undo (planned)

### **Real-time Execution**
- Visual feedback during workflow execution
- Node-by-node progress indication
- Execution time and performance metrics
- Error handling and debugging information

## 🔗 API Integration

### **AI Service Providers**

#### **OpenAI Integration**
```javascript
// Text generation configuration
{
  model: "gpt-4",
  maxTokens: 150,
  temperature: 0.7,
  systemPrompt: "You are a helpful assistant"
}
```

#### **Azure Cognitive Services**
```javascript
// Image analysis configuration
{
  provider: "azure",
  features: ["description", "tags", "objects"],
  customPrompt: "Describe this image in detail"
}
```

#### **Google Cloud AI**
```javascript
// Translation configuration
{
  provider: "google",
  sourceLanguage: "auto",
  targetLanguage: "es"
}
```

### **Automation Integrations**

#### **Zapier Webhooks**
```javascript
{
  webhookUrl: "https://hooks.zapier.com/hooks/catch/...",
  method: "POST",
  headers: { "Content-Type": "application/json" }
}
```

#### **Slack Bot Integration**
```javascript
{
  botToken: "xoxb-your-bot-token",
  defaultChannel: "#general",
  messageFormat: "markdown"
}
```

## 🎨 Customization

### **Styling and Themes**
The interface uses CSS custom properties for easy theming:

```css
:root {
  --primary-color: #007bff;
  --success-color: #28a745;
  --warning-color: #ffc107;
  --danger-color: #dc3545;
  --background-color: #f8f9fa;
  --text-color: #212529;
}
```

### **Adding Custom Components**
Extend the component library by adding to the `componentDefinitions` object:

```javascript
'custom-component': {
  name: 'Custom Component',
  description: 'Your custom functionality',
  icon: 'fas fa-custom-icon',
  category: 'custom',
  inputs: [{ id: 'input1', name: 'Input', type: 'string' }],
  outputs: [{ id: 'output1', name: 'Output', type: 'string' }],
  properties: { customSetting: 'default' }
}
```

## 🔒 Security Considerations

### **API Key Management**
- Store API keys securely using environment variables
- Never commit secrets to version control
- Use Replit's secret management for deployed applications
- Implement key rotation policies

### **Data Privacy**
- Process sensitive data according to compliance requirements
- Implement data encryption for stored workflows
- Provide data deletion capabilities
- Log and monitor data access

### **Authentication**
- JWT-based authentication for API access
- Role-based access control for workflow management
- Session management and timeout policies
- Secure password requirements

## 📊 Monitoring & Analytics

### **Execution Metrics**
- Workflow execution count and frequency
- Average execution time per component type
- Success/failure rates and error patterns
- Resource usage and performance optimization

### **User Analytics**
- Most popular component types
- Common workflow patterns
- User engagement and feature adoption
- Performance bottlenecks and optimization opportunities

## 🧪 Testing

### **Component Testing**
Each component type should be tested for:
- Input/output data validation
- Error handling and recovery
- Performance under load
- Integration with external services

### **Workflow Testing**
- End-to-end workflow execution
- Connection and data flow validation
- Complex logic and conditional paths
- Edge cases and error scenarios

## 🚀 Deployment

### **Replit Deployment**
The application is optimized for Replit deployment:

```bash
# Frontend served on port 5000
npm run dev

# Backend API configuration
# Database automatically configured
# Secrets managed through Replit interface
```

### **Production Deployment**
For production environments:

1. **Frontend**: Build and serve static files
2. **Backend**: Deploy .NET API to cloud platform
3. **Database**: Configure PostgreSQL with connection pooling
4. **Secrets**: Use cloud provider's secret management
5. **Monitoring**: Implement logging and alerting

## 🤝 Contributing

### **Development Setup**
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

### **Code Standards**
- Follow existing code style and patterns
- Add JSDoc comments for new functions
- Update documentation for new features
- Ensure responsive design for new UI elements

## 📋 Project Structure

```
visual-ai-workflow-designer/
├── visual-ai-workflow-designer.html    # Standalone complete application
├── README.md                          # This documentation
├── services/                          # Backend services
│   └── workflow/
│       └── WorkflowApi/              # .NET Core API
│           ├── Controllers/          # API endpoints
│           ├── Models/              # Data models
│           ├── Services/            # Business logic
│           └── Data/               # Database context
├── frontend/                         # Vue.js frontend (separate version)
│   ├── src/
│   │   ├── components/             # Vue components
│   │   ├── views/                  # Page components
│   │   └── services/               # API clients
│   └── public/                     # Static assets
└── database/
    └── schema.sql                  # Database schema
```

## 🏆 Use Cases

### **Business Process Automation**
- Customer support ticket routing
- Invoice processing and approval
- Employee onboarding workflows
- Quality assurance processes

### **Content Management**
- Social media content moderation
- Blog post generation and publishing
- Image processing and optimization
- Document classification and filing

### **Data Processing**
- Lead scoring and qualification
- Survey response analysis
- Report generation and distribution
- Data cleansing and validation

### **Integration & Communication**
- Multi-platform notification systems
- Data synchronization between systems
- Automated reporting and dashboards
- Event-driven workflow triggers

## 📞 Support

### **Documentation**
- Complete API documentation available
- Video tutorials for common workflows
- Best practices and design patterns
- Troubleshooting guides

### **Community**
- GitHub Issues for bug reports
- Feature requests and roadmap
- Community examples and templates
- Regular updates and improvements

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

---

**Built with ❤️ for the no-code community**

*Empowering everyone to build powerful AI workflows without coding barriers.*
