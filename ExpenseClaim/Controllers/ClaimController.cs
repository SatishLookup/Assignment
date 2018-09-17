using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExpenseClaim.Services.Contract;
using AutoMapper;
using ExpenseClaim.Modules;
using System.Xml.Linq;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;
using System.Text;
using System.Runtime.Serialization.Json;
using ExpenseClaim.Entities;
using Microsoft.Extensions.Logging;

namespace ExpenseClaim.Controllers
{
    [Route("api/claims")]
    public class ClaimController : Controller
    {
        private IClaimRepository _claimRepository;
        private IRemoveInvalidCharFromXML _removeInvalidCharFromXML;
        private readonly string defaultCostCenter;
        private ILogger<ClaimController> _logger;

        public ClaimController(IClaimRepository claimRepository, IRemoveInvalidCharFromXML removeInvalidCharFromXML,
                                ILogger<ClaimController> logger)
        {
            _claimRepository = claimRepository;
            _removeInvalidCharFromXML = removeInvalidCharFromXML;
            defaultCostCenter = _claimRepository.getDefaultCostCenter();
            _logger = logger;
               
        }

        [HttpGet()]
        public IActionResult GetExpenses()
        {
            _logger.LogInformation("Processing GetExpenses Request");
           var expensesFromDB = _claimRepository.GetExpenses();

            if (!(expensesFromDB?.Any() ?? false))
                return NotFound();

            var expenseDto = Mapper.Map<IEnumerable<ClaimsDto>>(expensesFromDB);

            _logger.LogInformation("Completed processing GetExpenses Request - {@expenseDto}");

            return Ok(expenseDto);
        }

        [HttpGet("{id}", Name = "GetExpense")]
        public IActionResult GetExpense(Guid id)
        {
            _logger.LogInformation("Processing GetExpense Request");
            var expenseFromDB = _claimRepository.GetExpense(id);

            if (expenseFromDB == null)
                return NotFound();

            var expenseDto = Mapper.Map<ClaimsDto>(expenseFromDB);

            _logger.LogInformation("Completed processing GetExpense Request - {@expenseDto}");

            return Ok(expenseDto);
        }

        [HttpGet("/CostCenter/{id}")]
        public IActionResult GetExpenseForCostCenter(string costcenterId)
        {
            _logger.LogInformation("Processing GetExpenseForCostCenter Request");
            var expenseFromDB = _claimRepository.GetExpenseForCostCenter(costcenterId);

            if (expenseFromDB == null)
                return NotFound();

            var expenseDto = Mapper.Map<ClaimsDto>(expenseFromDB);

            _logger.LogInformation("Completed processing GetExpenseForCostCenter Request - {@expenseDto}");

            return Ok(expenseDto);
        }

        [HttpPost()]
        public IActionResult CreateClaim([FromBody] string email)
        {
            _logger.LogInformation("Processing GetExpenseForCostCenter Request");
            string requestXML = _removeInvalidCharFromXML.GetCleanXML(email);
            ClaimsDto processClaim = new ClaimsDto();
            XDocument docx = XDocument.Parse(requestXML.ToString());

            processClaim.costCenter = docx.Descendants("cost_centre")?.FirstOrDefault().Value.Length > 0 ?
                                      docx.Descendants("cost_centre")?.FirstOrDefault().Value :
                                      "UNKNOWN" ;

            processClaim.total = docx.Descendants("total").FirstOrDefault().Value;
            processClaim.PaymentMethod = docx.Descendants("payment_method").FirstOrDefault().Value;
            processClaim.Description = docx.Descendants("description").FirstOrDefault().Value;
            processClaim.Vendor = docx.Descendants("vendor").FirstOrDefault().Value;
            processClaim.date = DateTime.Parse(docx.Descendants("date").FirstOrDefault().Value);

            //If user input is not valid - Send Status code 400 Bad request
            if (processClaim == null)
            {
                _logger.LogError("Invalid user request.  Sending 400 Bad Request");
                return BadRequest();
            }


            _logger.LogError("User request - {@processClaim}");
            TryValidateModel(processClaim);
            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid user request.  Sending 422 Sstatus Request");
                return new UnprocessableEntityObjectResult(ModelState);
            }

            var claimexpense = Mapper.Map<Expenses>(processClaim);
            _claimRepository.AddClaim(claimexpense);

            //Incase save fails, send 500 Status to the Client.
            if(!_claimRepository.Save())
            {
                _logger.LogError("Request failed to save {@claimexpense}");
                throw new Exception("Fail to add new Claim");
            }

            var claimToReturn = Mapper.Map<ClaimsDto>(claimexpense);

            _logger.LogError("Completed Request, Return status code 201");
            //Return url with status code 201 Created.  In postman under Header section there should be Location Url created
            return CreatedAtRoute("GetExpense", new { id = new Guid(claimToReturn.Id) }, claimToReturn);
        }
    }
}
