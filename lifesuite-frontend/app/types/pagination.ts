export interface Pagination {
    pageNumber: number,
    pageSize: number,
    totalCount: number,
    totalPages: number,
    offset: number,
}

export interface PaginatedList<T> {
    items: T[],
    pagination: Pagination
}