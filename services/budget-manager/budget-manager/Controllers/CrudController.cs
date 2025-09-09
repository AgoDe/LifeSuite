using BudgetManager.Data.Abstraction.Services;
using BudgetManager.Data.Abstraction.Validation;
using BudgetManager.Data.Exceptions;
using BudgetManager.Data.Models.Dto.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BudgetManager.Api.Models.Interfaces;
using BudgetManager.Api.Models;
using BudgetManager.Data.Abstraction.Models.Dto;
using System.Net;
using budget_manager.Filters;

namespace BudgetManager.Api.Controllers
{
    /// <summary>
    /// Controller CRUD generico astratto per operazioni standard
    /// </summary>
    /// <typeparam name="TDto">Tipo del DTO per operazioni di lettura</typeparam>
    /// <typeparam name="TFormDto">Tipo del DTO per operazioni di creazione/modifica</typeparam>
    /// <typeparam name="TFilter">Tipo del filtro per le query di lista</typeparam>
    [ApiController]
    [AutoSetUserId]
    [Route("api/[controller]")]
    public abstract class CrudController<TDto, TFormDto, TFilter> : ControllerBase
        where TDto : class, IBaseDto
        where TFormDto : class, IFormDto
        where TFilter : class, IListFilter
    {
        protected readonly ICrudService<TDto, TFormDto, TFilter> _service;

        protected CrudController(ICrudService<TDto, TFormDto, TFilter> service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Recupera un'entità per ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(IApiResponse), 200)]
        [ProducesResponseType(404)]
        public virtual async Task<IActionResult> GetById(Guid id)
        {
            ApiResponse response;
            try
            {
                var dto = await _service.GetByIdAsync(id);
                if (dto == null)
                {
                    response = new ApiResponse(ApiResponse.ApiResponseType.NotFound, "Entity not found");
                    return NotFound(response);
                }

                response = new ApiResponse<TDto>(dto);
            }
            catch (Exception ex)
            {
                response = new ApiResponse(ApiResponse.ApiResponseType.Error, ex.Message);
            }

            return Ok(response);
        }

        /// <summary>
        /// Recupera una lista paginata di entità con filtri
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IApiResponse), 200)]
        public virtual async Task<IActionResult> GetList([FromQuery] TFilter filter)
        {
            ApiResponse response;
            try
            {
                var result = await _service.GetListAsync(filter);
                response = new ApiResultResponse(result);
            }
            catch (Exception ex)
            {
                response = new ApiResponse(ApiResponse.ApiResponseType.Error, ex.Message);
            }

            return Ok(response);
        }

        /// <summary>
        /// Crea una nuova entità
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(IApiResponse), 201)]
        [ProducesResponseType(typeof(IApiResponse), 400)]
        public virtual async Task<IActionResult> Create([FromBody] TFormDto formDto)
        {
            ApiResponse response;
            try
            {
                var id = await _service.CreateAsync(formDto);
                response = new ApiResponse<Guid>(id) { StatusCode = HttpStatusCode.Created };
                return CreatedAtAction(nameof(GetById), new { id }, response);
            }
            catch (ValidationException ex)
            {
                var validationResult = new ValidationResult { IsValid = false, Errors = ex.Errors.ToList() };
                response = new ApiResultResponse(validationResult) { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
                return BadRequest(response);
            }
            catch (BusinessRuleException ex)
            {
                response = new ApiResponse(ApiResponse.ApiResponseType.Error, ex.Message);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Aggiorna un'entità esistente
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(IApiResponse), 204)]
        [ProducesResponseType(typeof(IApiResponse), 400)]
        [ProducesResponseType(typeof(IApiResponse), 404)]
        public virtual async Task<IActionResult> Update(Guid id, [FromBody] TFormDto formDto)
        {
            ApiResponse response;
            try
            {
                await _service.UpdateAsync(id, formDto);
                response = new ApiResponse(ApiResponse.ApiResponseType.NoContent, "Entity updated successfully");
                return Ok(response);
            }
            catch (EntityNotFoundException)
            {
                response = new ApiResponse(ApiResponse.ApiResponseType.NotFound, "Entity not found");
                return NotFound(response);
            }
            catch (ValidationException ex)
            {
                var validationResult = new ValidationResult { IsValid = false, Errors = ex.Errors.ToList() };
                response = new ApiResultResponse(validationResult) { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
                return BadRequest(response);
            }
            catch (BusinessRuleException ex)
            {
                response = new ApiResponse(ApiResponse.ApiResponseType.Error, ex.Message);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Elimina un'entità
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(IApiResponse), 204)]
        [ProducesResponseType(typeof(IApiResponse), 404)]
        [ProducesResponseType(typeof(IApiResponse), 400)]
        public virtual async Task<IActionResult> Delete(Guid id)
        {
            ApiResponse response;
            try
            {
                await _service.DeleteAsync(id);
                response = new ApiResponse(ApiResponse.ApiResponseType.NoContent, "Entity deleted successfully");
                return Ok(response);
            }
            catch (EntityNotFoundException)
            {
                response = new ApiResponse(ApiResponse.ApiResponseType.NotFound, "Entity not found");
                return NotFound(response);
            }
            catch (BusinessRuleException ex)
            {
                response = new ApiResponse(ApiResponse.ApiResponseType.Error, ex.Message);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Verifica se un'entità esiste
        /// </summary>
        [HttpHead("{id}")]
        [ProducesResponseType(typeof(IApiResponse), 200)]
        [ProducesResponseType(typeof(IApiResponse), 404)]
        public virtual async Task<IActionResult> Exists(Guid id)
        {
            ApiResponse response;
            try
            {
                var exists = await _service.ExistsAsync(id);
                if (exists)
                {
                    response = new ApiResponse(ApiResponse.ApiResponseType.Success, "Entity exists");
                    return Ok(response);
                }
                else
                {
                    response = new ApiResponse(ApiResponse.ApiResponseType.NotFound, "Entity not found");
                    return NotFound(response);
                }
            }
            catch (Exception ex)
            {
                response = new ApiResponse(ApiResponse.ApiResponseType.Error, ex.Message);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Conta il numero totale di entità che soddisfano i filtri
        /// </summary>
        [HttpGet("count")]
        [ProducesResponseType(typeof(IApiResponse), 200)]
        public virtual async Task<IActionResult> Count([FromQuery] TFilter filter)
        {
            ApiResponse response;
            try
            {
                var count = await _service.CountAsync(filter);
                response = new ApiResponse<int>(count);
            }
            catch (Exception ex)
            {
                response = new ApiResponse(ApiResponse.ApiResponseType.Error, ex.Message);
            }

            return Ok(response);
        }

        /// <summary>
        /// Valida un FormDTO senza salvarlo
        /// </summary>
        [HttpPost("validate")]
        [ProducesResponseType(typeof(IApiResponse), 200)]
        public virtual async Task<IActionResult> Validate([FromBody] TFormDto formDto)
        {
            ApiResponse response;
            try
            {
                var result = await _service.ValidateWithDetailsAsync(formDto);
                response = new ApiResultResponse(result);
            }
            catch (Exception ex)
            {
                response = new ApiResponse(ApiResponse.ApiResponseType.Error, ex.Message);
            }

            return Ok(response);
        }

        /// <summary>
        /// Crea multiple entità in batch
        /// </summary>
        [HttpPost("batch")]
        [ProducesResponseType(typeof(IApiResponse), 201)]
        [ProducesResponseType(typeof(IApiResponse), 400)]
        public virtual async Task<IActionResult> CreateMany([FromBody] IEnumerable<TFormDto> formDtos)
        {
            ApiResponse response;
            try
            {
                var ids = await _service.CreateManyAsync(formDtos);
                response = new ApiResultResponse(ids) { StatusCode = HttpStatusCode.Created };
                return Created(string.Empty, response);
            }
            catch (ValidationException ex)
            {
                var validationResult = new ValidationResult { IsValid = false, Errors = ex.Errors.ToList() };
                response = new ApiResultResponse(validationResult) { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };
                return BadRequest(response);
            }
            catch (BusinessRuleException ex)
            {
                response = new ApiResponse(ApiResponse.ApiResponseType.Error, ex.Message);
                return BadRequest(response);
            }
        }

        /// <summary>
        /// Elimina multiple entità in batch
        /// </summary>
        [HttpDelete("batch")]
        [ProducesResponseType(typeof(IApiResponse), 204)]
        [ProducesResponseType(typeof(IApiResponse), 404)]
        [ProducesResponseType(typeof(IApiResponse), 400)]
        public virtual async Task<IActionResult> DeleteMany([FromBody] IEnumerable<Guid> ids)
        {
            ApiResponse response;
            try
            {
                await _service.DeleteManyAsync(ids);
                response = new ApiResponse(ApiResponse.ApiResponseType.NoContent, "Entities deleted successfully");
                return Ok(response);
            }
            catch (EntityNotFoundException ex)
            {
                response = new ApiResponse(ApiResponse.ApiResponseType.NotFound, ex.Message);
                return NotFound(response);
            }
            catch (BusinessRuleException ex)
            {
                response = new ApiResponse(ApiResponse.ApiResponseType.Error, ex.Message);
                return BadRequest(response);
            }
        }
    }
}