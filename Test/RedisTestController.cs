using Microsoft.AspNetCore.Mvc;

namespace Test
{
    //[ApiController]
    //[Route("api/[controller]")]
    //public class RedisTestController : ControllerBase
    //{
    //    private readonly IRedisCacheService _redisCacheService;

    //    public RedisTestController(IRedisCacheService redisCacheService)
    //    {
    //        _redisCacheService = redisCacheService;
    //    }

    //    // Lưu dữ liệu vào Redis
    //    [HttpPost("set")]
    //    public async Task<IActionResult> SetData([FromQuery] string key, [FromBody] object value)
    //    {
    //        await _redisCacheService.SetData(key, value, TimeSpan.FromMinutes(10)); // Lưu với thời gian hết hạn là 10 phút
    //        return Ok(new { Message = "Data saved successfully", Key = key, Value = value });
    //    }

    //    // Lấy dữ liệu từ Redis
    //    [HttpGet("get")]
    //    public async Task<IActionResult> GetData([FromQuery] string key)
    //    {
    //        var data = await _redisCacheService.GetData<object>(key);
    //        if (data == null)
    //        {
    //            return NotFound(new { Message = "Data not found", Key = key });
    //        }
    //        return Ok(new { Message = "Data retrieved successfully", Key = key, Value = data });
    //    }

    //    // Xóa dữ liệu khỏi Redis
    //    [HttpDelete("remove")]
    //    public async Task<IActionResult> RemoveData([FromQuery] string key)
    //    {
    //        await _redisCacheService.RemoveData(key);
    //        return Ok(new { Message = "Data removed successfully", Key = key });
    //    }
    //}
}
