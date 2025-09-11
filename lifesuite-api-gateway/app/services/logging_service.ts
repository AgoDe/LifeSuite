import { randomUUID } from 'crypto'
import type { 
  LogContext, 
  LogLevel, 
  StructuredLog, 
  MicroserviceLogData, 
  RequestLogData, 
  ErrorLogData 
} from '../types/logging.js'

export class LoggingService {
  private static instance: LoggingService
  private isDevelopment: boolean

  constructor() {
    this.isDevelopment = process.env.NODE_ENV === 'development'
  }

  static getInstance(): LoggingService {
    if (!LoggingService.instance) {
      LoggingService.instance = new LoggingService()
    }
    return LoggingService.instance
  }

  /**
   * Genera un correlation ID unico per tracciare richieste
   */
  generateCorrelationId(): string {
    return randomUUID()
  }

  /**
   * Crea il context base per i log
   */
  createContext(overrides: Partial<LogContext> = {}): LogContext {
    return {
      correlationId: this.generateCorrelationId(),
      timestamp: new Date().toISOString(),
      ...overrides
    }
  }

  /**
   * Log generico strutturato
   */
  private log(level: LogLevel, message: string, context: LogContext, data?: any): void {
    const logEntry: StructuredLog = {
      level,
      message,
      timestamp: new Date().toISOString(),
      context,
      ...(data && { data })
    }

    if (this.isDevelopment) {
      // In sviluppo: output colorato e leggibile
      this.logDevelopment(logEntry)
    } else {
      // In produzione: JSON strutturato per parsing automatico
      console.log(JSON.stringify(logEntry))
    }
  }

  /**
   * Formattazione per sviluppo
   */
  private logDevelopment(logEntry: StructuredLog): void {
    const colors : any = {
      error: '\x1b[31m',   // Rosso
      warn: '\x1b[33m',    // Giallo
      info: '\x1b[36m',    // Ciano
      debug: '\x1b[90m',   // Grigio
      reset: '\x1b[0m'
    }

    const color = colors[logEntry.level] || colors.reset
    const levelUpper = logEntry.level.toUpperCase().padEnd(5)
    
    console.log(`${color}[${levelUpper}]${colors.reset} ${logEntry.message}`)
    console.log(`  📍 Correlation: ${logEntry.context.correlationId}`)
    
    if (logEntry.context.userId) {
      console.log(`  👤 User: ${logEntry.context.userId}`)
    }
    
    if (logEntry.context.endpoint) {
      console.log(`  🌐 ${logEntry.context.method} ${logEntry.context.endpoint}`)
    }
    
    if (logEntry.data) {
      console.log(`  📋 Data:`, logEntry.data)
    }
    
    console.log() // Riga vuota per separazione
  }

  /**
   * Log richieste HTTP in arrivo
   */
  logRequest(data: RequestLogData): void {
    const context = this.createContext({
      correlationId: data.correlationId,
      userId: data.userId,
      method: data.method,
      endpoint: data.url,
      userAgent: data.userAgent,
      ip: data.ip
    })

    const message = `HTTP Request: ${data.method} ${data.url}`
    
    this.log('info', message, context, {
      duration: data.duration,
      statusCode: data.statusCode
    })
  }

  /**
   * Log chiamate ai microservizi
   */
  logMicroserviceCall(data: MicroserviceLogData): void {
    const context = this.createContext({
      correlationId: data.correlationId,
      userId: data.userId?.toString(),
      service: data.microservice,
      method: data.method,
      endpoint: data.endpoint
    })

    const message = `Microservice Call: ${data.microservice} ${data.method.toUpperCase()} ${data.endpoint}`
    
    const logData = {
      requestData: data.requestData,
      responseStatus: data.responseStatus,
      duration: data.duration ? `${data.duration}ms` : undefined,
      ...(data.error && { error: data.error.message })
    }

    const level: LogLevel = data.error ? 'error' : 'info'
    this.log(level, message, context, logData)
  }

  /**
   * Log errori strutturati
   */
  logError(data: ErrorLogData): void {
    const message = `Error: ${data.error.message}`
    
    const logData = {
      errorName: data.error.name,
      stack: data.stack || data.error.stack,
      statusCode: data.statusCode,
      ...data.additionalData
    }

    this.log('error', message, data.context, logData)
  }

  /**
   * Log eventi di autenticazione
   */
  logAuth(event: 'login' | 'logout' | 'unauthorized', context: LogContext, data?: any): void {
    const message = `Auth Event: ${event}`
    this.log('info', message, context, data)
  }

  /**
   * Log performance metrics
   */
  logPerformance(operation: string, duration: number, context: LogContext, data?: any): void {
    const message = `Performance: ${operation} completed in ${duration}ms`
    const level: LogLevel = duration > 1000 ? 'warn' : 'info'
    
    this.log(level, message, context, { duration, ...data })
  }

  // Metodi di convenienza
  info(message: string, context: LogContext, data?: any): void {
    this.log('info', message, context, data)
  }

  warn(message: string, context: LogContext, data?: any): void {
    this.log('warn', message, context, data)
  }

  error(message: string, context: LogContext, data?: any): void {
    this.log('error', message, context, data)
  }

  debug(message: string, context: LogContext, data?: any): void {
    this.log('debug', message, context, data)
  }
}

// Export singleton instance
export const logger = LoggingService.getInstance()