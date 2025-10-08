import type { PaginatedList } from "../../layers/budget-manager/types/budget-manager"

export interface ApiResponse {
    isSuccess: boolean
    message: string
    statusCode: string
}

export interface ApiDataResponse<T> extends ApiResponse {
    result: T
}

export interface ApiPaginatedListResponse<T> extends ApiResponse {
   result: PaginatedList<T>
}
