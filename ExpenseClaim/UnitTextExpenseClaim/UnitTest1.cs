using System;
using Xunit;
using System.Collections.Generic;
using ExpenseClaim.Controllers;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using ExpenseClaim.Services.Contract;
using ExpenseClaim.Services;
using ExpenseClaim.Modules;
using ExpenseClaim.Entities;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace UnitTextExpenseClaim
{
    public class UnitTest1
    {
        private IList<Expenses> mockExpenseResult;

        public UnitTest1()
        {

            AutoMapper.Mapper.Initialize(cfg =>
            {
                IGST gST = new GST();
                cfg.CreateMap<Expenses, ClaimsDto>()
                .ForMember(dest => dest.costCenter, opt => opt.MapFrom(src => src.CostCenterId))
                .ForMember(dest => dest.date, opt => opt.MapFrom(src => src.TransactionDate))
                .ForMember(dest => dest.total, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.gstCalculated, opt => opt.MapFrom(src => gST.CalculateGST(src.TotalAmount)));

                cfg.CreateMap<ClaimsDto, Expenses>()
                .ForMember(dest => dest.CostCenterId, opt => opt.MapFrom(src => src.costCenter))
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.date))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.total))
                .ForMember(dest => dest.CostCenter, opt => opt.Ignore());
            });

            mockExpenseResult = GetTestSessionExpense();
            
        }


        [Fact]
        public void ExpenseClaim_ClaimController_GetExpenses_ShouldGetExpense()
        {
            var mockInvalidxMLChar = new Mock<IRemoveInvalidCharFromXML>();
            var mockClaimRepository = new Mock<IClaimRepository>();
            mockClaimRepository.Setup(a => a.GetExpenses()).Returns(GetTestSessionExpense());
            var mockLogger = Mock.Of<ILogger<ClaimController>>();

            var controller = new ClaimController(mockClaimRepository.Object, mockInvalidxMLChar.Object, mockLogger);
            var actionResult = controller.GetExpenses();

            Assert.IsType<OkObjectResult>(actionResult);
        }

        [Fact]
        public void ExpenseClaim_ClaimController_GetExpense_ShouldGetExpense()
        {
            var mockInvalidxMLChar = new Mock<IRemoveInvalidCharFromXML>();
            var expenseGuid = mockExpenseResult.FirstOrDefault().Id;
            var mockClaimRepository = new Mock<IClaimRepository>();
            mockClaimRepository.Setup(a => a.GetExpense(expenseGuid)).Returns(mockExpenseResult.FirstOrDefault()).Verifiable();
            var mockLogger = Mock.Of<ILogger<ClaimController>>();

            var controller = new ClaimController(mockClaimRepository.Object, mockInvalidxMLChar.Object, mockLogger);
            var actionResult = controller.GetExpense(expenseGuid);

            Assert.IsType<OkObjectResult>(actionResult);
        }

        [Fact]
        public void ExpenseClaim_ClaimController_POSTExpense_ShouldGetCreated()
        {
            var mockInvalidxMLChar = new Mock<IRemoveInvalidCharFromXML>();
            var mockClaimRepository = new Mock<IClaimRepository>();
            IRemoveInvalidCharFromXML removeXml = new RemoveInvalidCharFromXML();
            var mockLogger = Mock.Of<ILogger<ClaimController>>();
            mockClaimRepository.Setup(a => a.AddClaim(mockExpenseResult.FirstOrDefault()));
            mockClaimRepository.Setup(a => a.Save()).Returns(true);

            mockInvalidxMLChar.Setup(a => a.GetCleanXML(GetTestSessionExpenseEmail())).Returns(removeXml.GetCleanXML(GetTestSessionExpenseEmail()));

            var controller = new ClaimController(mockClaimRepository.Object, mockInvalidxMLChar.Object, mockLogger);

            var objectValidator = new Mock<IObjectModelValidator>();
            objectValidator.Setup(o => o.Validate(It.IsAny<ActionContext>(),
                                              It.IsAny<ValidationStateDictionary>(),
                                              It.IsAny<string>(),
                                              It.IsAny<Object>()));
            controller.ObjectValidator = objectValidator.Object;

            var actionResult = controller.CreateClaim(GetTestSessionExpenseEmail());
            var okResult = actionResult as CreatedAtRouteResult;

            Assert.Equal(201, okResult.StatusCode);
        }

        private IList<Expenses> GetTestSessionExpense()
        {
            var expenses = new List<Expenses>()
            {
                new Expenses(){ Id= Guid.NewGuid(), CostCenterId = "DEV002", TotalAmount = 1000, Gst= 100,
                                TransactionDate = DateTime.Parse("1 jan 2018"), Description="Cost for DEv",
                                Vendor = "Vendor1", CreatedAt= DateTime.Parse("12 Sep 2018"), PaymentMethod="CARD"  },

                new Expenses(){ Id= Guid.NewGuid(), CostCenterId = "DEV002", TotalAmount = 2000, Gst= 200,
                                TransactionDate = DateTime.Parse("10 jan 2018"), Description = "Cost for Dev2",
                                Vendor = "Vendor2",  CreatedAt= DateTime.Parse("12 Sep 2018"), PaymentMethod = "CASH"  }
            };

            return expenses.ToList();
        }

        private string GetTestSessionExpenseEmail()
        {
            string email = @"Hi Yvaine,
                            Please create an expense claim for the below. Relevant details are marked up as
                            requested…
                            <expense><cost_centre></cost_centre>
                            <total>1024.01</total><payment_method>personal card</payment_method>
                            </expense>
                            From: Ivan Castle
                            Sent: Friday, 16 February 2018 10:32 AM
                            To: Antoine Lloyd <Antoine.Lloyd@example.com>
                            Subject: test
                            Hi Antoine,
                            Please create a reservation at the <vendor>Viaduct Steakhouse</vendor> our
                            <description>development team’s project end celebration dinner</description> on
                            <date>Tuesday 25 April 2017</date>. We expect to arrive around
                            7.15pm. Approximately 12 people but I’ll confirm exact numbers closer to the day.
                            Regards,
                            Ivan";

            return email;
        }


        
    }
}
