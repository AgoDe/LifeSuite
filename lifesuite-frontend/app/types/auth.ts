// Authentication types
export interface User {
  id: number
  email: string
  fullName: string
  avatar?: string
  createdAt: string
  updatedAt: string
}

export interface LoginCredentials {
  email: string
  password: string
}

export interface RegisterData {
  firstName: string
  lastName: string
  email: string
  password: string
}

export interface AuthResponse {
  user: User
  token: string
}