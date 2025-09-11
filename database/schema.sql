-- Visual AI Workflow Database Schema
-- This schema supports the low-code AI workflow platform

-- Create database (PostgreSQL)
-- CREATE DATABASE WorkflowDB;
-- \c WorkflowDB;

-- Enable UUID extension
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Users table
CREATE TABLE Users (
    Id SERIAL PRIMARY KEY,
    Username VARCHAR(100) UNIQUE NOT NULL,
    Email VARCHAR(255) UNIQUE NOT NULL,
    PasswordHash VARCHAR(500) NOT NULL,
    FirstName VARCHAR(100),
    LastName VARCHAR(100),
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    LastLoginAt TIMESTAMP,
    IsActive BOOLEAN DEFAULT true
);

-- Workflows table
CREATE TABLE Workflows (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    Description TEXT,
    Definition TEXT NOT NULL DEFAULT '{}', -- JSON representation of workflow
    Version INTEGER DEFAULT 1,
    IsActive BOOLEAN DEFAULT true,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UserId INTEGER NOT NULL REFERENCES Users(Id) ON DELETE CASCADE
);

-- Workflow executions table
CREATE TABLE WorkflowExecutions (
    Id SERIAL PRIMARY KEY,
    WorkflowId INTEGER NOT NULL REFERENCES Workflows(Id) ON DELETE CASCADE,
    Status INTEGER NOT NULL DEFAULT 0, -- 0=Pending, 1=Running, 2=Completed, 3=Failed, 4=Cancelled
    StartedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CompletedAt TIMESTAMP,
    Input TEXT DEFAULT '{}', -- JSON input data
    Output TEXT DEFAULT '{}', -- JSON output data
    ErrorMessage TEXT,
    ExecutionLog TEXT DEFAULT '[]', -- JSON array of log entries
    ExecutionTimeMs INTEGER DEFAULT 0
);

-- API connections for external services
CREATE TABLE ApiConnections (
    Id SERIAL PRIMARY KEY,
    UserId INTEGER NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
    Name VARCHAR(200) NOT NULL,
    Provider VARCHAR(100) NOT NULL, -- 'openai', 'azure', 'google', etc.
    ConnectionType VARCHAR(50) NOT NULL, -- 'ai', 'automation', 'database'
    Configuration TEXT NOT NULL DEFAULT '{}', -- JSON config (encrypted credentials)
    IsActive BOOLEAN DEFAULT true,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UpdatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Workflow templates
CREATE TABLE WorkflowTemplates (
    Id SERIAL PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    Description TEXT,
    Category VARCHAR(100), -- 'AI', 'Automation', 'Data Processing'
    Icon VARCHAR(100), -- CSS class for icon
    Definition TEXT NOT NULL DEFAULT '{}', -- JSON workflow definition
    Tags TEXT DEFAULT '[]', -- JSON array of tags
    IsPublic BOOLEAN DEFAULT false,
    CreatedById INTEGER REFERENCES Users(Id),
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UsageCount INTEGER DEFAULT 0
);

-- Audit log for tracking changes
CREATE TABLE AuditLogs (
    Id SERIAL PRIMARY KEY,
    UserId INTEGER REFERENCES Users(Id),
    EntityType VARCHAR(100) NOT NULL, -- 'Workflow', 'Execution', etc.
    EntityId INTEGER NOT NULL,
    Action VARCHAR(50) NOT NULL, -- 'CREATE', 'UPDATE', 'DELETE', 'EXECUTE'
    OldValues TEXT, -- JSON of old values
    NewValues TEXT, -- JSON of new values
    Timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    IpAddress INET,
    UserAgent TEXT
);

-- Webhook endpoints for external triggers
CREATE TABLE WebhookEndpoints (
    Id SERIAL PRIMARY KEY,
    WorkflowId INTEGER NOT NULL REFERENCES Workflows(Id) ON DELETE CASCADE,
    Token UUID DEFAULT uuid_generate_v4(),
    Name VARCHAR(200),
    IsActive BOOLEAN DEFAULT true,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    LastTriggeredAt TIMESTAMP,
    TriggerCount INTEGER DEFAULT 0
);

-- Scheduled workflow runs
CREATE TABLE ScheduledRuns (
    Id SERIAL PRIMARY KEY,
    WorkflowId INTEGER NOT NULL REFERENCES Workflows(Id) ON DELETE CASCADE,
    CronExpression VARCHAR(100) NOT NULL, -- Cron format for scheduling
    NextRunAt TIMESTAMP,
    LastRunAt TIMESTAMP,
    IsActive BOOLEAN DEFAULT true,
    CreatedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FailureCount INTEGER DEFAULT 0,
    MaxRetries INTEGER DEFAULT 3
);

-- Workflow sharing and collaboration
CREATE TABLE WorkflowShares (
    Id SERIAL PRIMARY KEY,
    WorkflowId INTEGER NOT NULL REFERENCES Workflows(Id) ON DELETE CASCADE,
    SharedWithUserId INTEGER NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
    Permission VARCHAR(20) NOT NULL DEFAULT 'READ', -- 'read', 'write', 'execute'
    SharedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    SharedById INTEGER NOT NULL REFERENCES Users(Id),
    UNIQUE(WorkflowId, SharedWithUserId)
);

-- Execution step details (for detailed logging)
CREATE TABLE ExecutionSteps (
    Id SERIAL PRIMARY KEY,
    ExecutionId INTEGER NOT NULL REFERENCES WorkflowExecutions(Id) ON DELETE CASCADE,
    NodeId VARCHAR(100) NOT NULL, -- Node ID from workflow definition
    NodeType VARCHAR(100) NOT NULL,
    Status INTEGER NOT NULL DEFAULT 0, -- 0=Pending, 1=Running, 2=Completed, 3=Failed
    StartedAt TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CompletedAt TIMESTAMP,
    Input TEXT DEFAULT '{}',
    Output TEXT DEFAULT '{}',
    ErrorMessage TEXT,
    ExecutionTimeMs INTEGER DEFAULT 0,
    StepOrder INTEGER NOT NULL
);

-- Create indexes for better performance
CREATE INDEX idx_workflows_user_id ON Workflows(UserId);
CREATE INDEX idx_workflows_updated_at ON Workflows(UpdatedAt DESC);
CREATE INDEX idx_workflow_executions_workflow_id ON WorkflowExecutions(WorkflowId);
CREATE INDEX idx_workflow_executions_started_at ON WorkflowExecutions(StartedAt DESC);
CREATE INDEX idx_workflow_executions_status ON WorkflowExecutions(Status);
CREATE INDEX idx_api_connections_user_id ON ApiConnections(UserId);
CREATE INDEX idx_api_connections_provider ON ApiConnections(Provider);
CREATE INDEX idx_audit_logs_entity ON AuditLogs(EntityType, EntityId);
CREATE INDEX idx_audit_logs_timestamp ON AuditLogs(Timestamp DESC);
CREATE INDEX idx_webhook_endpoints_token ON WebhookEndpoints(Token);
CREATE INDEX idx_scheduled_runs_next_run ON ScheduledRuns(NextRunAt) WHERE IsActive = true;
CREATE INDEX idx_execution_steps_execution_id ON ExecutionSteps(ExecutionId);
CREATE INDEX idx_execution_steps_order ON ExecutionSteps(ExecutionId, StepOrder);

-- Create trigger to update UpdatedAt timestamp
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
   NEW.UpdatedAt = CURRENT_TIMESTAMP;
   RETURN NEW;
END;
$$ language 'plpgsql';

CREATE TRIGGER update_workflows_updated_at BEFORE UPDATE ON Workflows
FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_api_connections_updated_at BEFORE UPDATE ON ApiConnections
FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- Insert sample user for development
INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName) 
VALUES ('admin', 'admin@example.com', 'hashed_password_here', 'Admin', 'User');

-- Insert sample workflow templates
INSERT INTO WorkflowTemplates (Name, Description, Category, Icon, Definition, Tags, IsPublic) VALUES
('AI Customer Support', 'Automated customer support with sentiment analysis and smart routing', 'AI', 'fas fa-headset', 
 '{"nodes": [{"type": "data-input", "name": "Customer Message"}, {"type": "ai-sentiment", "name": "Analyze Sentiment"}, {"type": "logic-condition", "name": "Route by Sentiment"}, {"type": "ai-text", "name": "Generate Response"}, {"type": "automation-email", "name": "Send Response"}], "edges": []}',
 '["AI", "Customer Service", "Automation"]', true),

('Content Moderation', 'Automatically moderate user-generated content using AI', 'AI', 'fas fa-shield-alt',
 '{"nodes": [{"type": "data-input", "name": "User Content"}, {"type": "ai-text", "name": "Content Analysis"}, {"type": "logic-condition", "name": "Safety Check"}, {"type": "automation-slack", "name": "Alert Moderators"}], "edges": []}',
 '["AI", "Moderation", "Safety"]', true),

('Document Processing', 'Extract and process information from documents', 'AI', 'fas fa-file-alt',
 '{"nodes": [{"type": "data-input", "name": "Document Upload"}, {"type": "ai-image", "name": "OCR Extraction"}, {"type": "ai-text", "name": "Information Extraction"}, {"type": "data-transform", "name": "Format Data"}, {"type": "data-output", "name": "Results"}], "edges": []}',
 '["AI", "OCR", "Documents"]', true),

('Social Media Monitor', 'Monitor social media mentions and respond automatically', 'Automation', 'fas fa-hashtag',
 '{"nodes": [{"type": "automation-zapier", "name": "Social Media Trigger"}, {"type": "ai-sentiment", "name": "Analyze Mention"}, {"type": "logic-condition", "name": "Filter Important"}, {"type": "automation-slack", "name": "Notify Team"}], "edges": []}',
 '["Social Media", "Monitoring", "AI"]', true);

-- Grant permissions (adjust as needed for your environment)
-- GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO workflow_app;
-- GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO workflow_app;
