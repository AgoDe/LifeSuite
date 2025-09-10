// Tipi per logging strutturato
export interface LogContext {
  correlationId: string
  userId?: string | number
  requestId?: string
  service?: string
  method?: string
  endpoint?: string
  userAgent?: string
  ip?: string
  timestamp: string
}

export interface MicroserviceLogData {
  microservice: string
  endpoint: string
  method: string
  userId: string | number
  correlationId: string
  requestData?: any
  responseStatus?: number
  responseData?: any
  duration?: number
  error?: any
}

export interface RequestLogData {
  method: string
  url: string
  correlationId: string
  userId?: string | number
  userAgent?: string
  ip?: string
  duration?: number
  statusCode?: number
  error?: any
}

export interface ErrorLogData {
  error: Error
  context: LogContext
  stack?: string
  statusCode?: number
  additionalData?: any
}

export type LogLevel = 'error' | 'warn' | 'info' | 'debug'

export interface StructuredLog {
  level: LogLevel
  message: string
  timestamp: string
  context: LogContext
  data?: any
}