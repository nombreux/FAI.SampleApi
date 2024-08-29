using Microsoft.AspNetCore.Mvc;
using SampleAPI.Entities;
using SampleAPI.Repositories;
using SampleAPI.Requests;

namespace SampleAPI.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        // Add more dependencies as needed.
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderRepository orderRepository, ILogger<OrdersController> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;

        }

        /// <summary>
        /// Retrives recent orders, that is less than a day old and has not been deleted.
        /// </summary>
        /// <returns>List of recent orders.</returns>
        /// <response code="200">Response when orders are successfully retrieved.</response>
        /// <response code="500">Response for server error.</response>
        /// 
        [HttpGet("recent")]
        [ProducesResponseType(StatusCodes.Status200OK)] 
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] 
        public async Task<ActionResult<List<Order>>> GetRecentOrders()
        {
            try
            {
                var orders = _orderRepository.GetRecentOrders();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting recent orders.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving orders.");
            }
        }

        /// TODO: Add an endpoint to allow users to create an order using <see cref="CreateOrderRequest"/>.
        /// 
        /// <summary>
        /// Creates a new order based on the provided request data.
        /// </summary>
        /// <param name="request">The data to create the order.</param>
        /// <returns>The newly created order.</returns>
        /// <response code="201">Response when the order is successfully created.</response>
        /// <response code="400">Response when the request is invalid.</response>
        /// <response code="500">Response for server error.</response>
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created)] 
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]  
        public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Name) || string.IsNullOrEmpty(request.Description))
            {
                _logger.LogWarning("Invalid create order request.");
                return BadRequest("Order name and description are required.");
            }

            try
            {
                var newOrder = new Order
                {
                    Name = request.Name,
                    Description = request.Description,
                    IsInvoiced = request.IsInvoiced ,
                };
                _orderRepository.AddOrder(newOrder);

                _logger.LogInformation("Order created successfully with ID {Id}.", newOrder.Id);

                return CreatedAtAction(nameof(GetRecentOrders), new { id = newOrder.Id }, newOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a new order.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the order.");
            }
        }

        /// <summary>
        /// Retrieves all orders submitted within the specified number of business days, excluding weekends and holidays.
        /// </summary>
        /// <param name="days">The number of business days to look back from today.</param>
        /// <returns>A list of orders submitted within the specified number of business days.</returns>
        /// <response code="200">Returns the list of orders that match the criteria.</response>
        /// <response code="400">If the provided number of days is invalid.</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpGet("businessdays/{days}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<Order>>> GetOrdersWithinBusinessDays(int days)
        {
            if (days <= 0)
            {
                _logger.LogWarning("Invalid number of days provided: {Days}", days);
                return BadRequest("The number of days must be greater than zero.");
            }

            try
            {
                var orders = _orderRepository.GetOrdersWithinBusinessDays(days);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting orders within business days.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving orders.");
            }
        }

    }
}
