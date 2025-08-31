using Application.IService;
using Microsoft.AspNetCore.Mvc;
using Domian.Response;
using Domain.Entities.CRUDEntities;

namespace CRUD_WEB_API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CrudController : ControllerBase
    {
        public readonly ICrudService crudData;

        public CrudController(ICrudService crudService)
        {
            crudData = crudService;
        }



        [HttpPost]
        [Route("StudentDataEntry")]
        public IActionResult StudentDataEntry([FromForm] StudentData studentData)
        {
            try
            {
                if (studentData == null)
                {
                    return BadRequest("Invalid data. Please provide valid Student information.");
                }
                var memberStatus = crudData.StudentDataEntry(studentData);
                ResponseMessage responseMessage = new ResponseMessage();
                responseMessage.data = "Response";
                responseMessage.ResponseObj = "Successfully Fetch";
                responseMessage.StatusCode = memberStatus.status;
                responseMessage.Message = memberStatus.message[0];
                return Ok(responseMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }







    }
}