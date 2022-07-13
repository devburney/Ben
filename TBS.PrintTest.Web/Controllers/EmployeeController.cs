using GoC.WebTemplate.Components.Core.Services;
using TBS.PrintTest.Business.Services.Interfaces;
using TBS.PrintTest.Web.Infrastructure.Services.Interfaces;
using TBS.PrintTest.Web.Models;
using TBS.PrintTest.Web.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using AutoMapper;
using TBS.PrintTest.Business.ErrorHandling;
using TBS.PrintTest.Business.Models;
using TBS.PrintTest.Web.Infrastructure.Helpers;
using System.Linq;



namespace TBS.PrintTest.Web.Controllers
{
    /// <summary>
    /// Employee controller
    /// </summary>
    public class EmployeeController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly ICodelookupService _codelookupService;
        private readonly IEmployeeService _employeeService;

        /// <summary>
        /// Constructor for employee controller
        /// </summary>
        /// <param name="modelAccessor">GoC Template model accessor</param>
        /// <param name="sitemapService">Sitemap service</param>
        /// <param name="logger">logger</param>
        /// <param name="codelookupService">codelookup service</param>
        /// <param name="employeeService">employee service</param>
        /// <param name="mapper">automapper</param>
        public EmployeeController(ModelAccessor modelAccessor, ISitemapService sitemapService, ILogger<EmployeeController> logger, 
                                  ICodelookupService codelookupService, IEmployeeService employeeService, IMapper mapper) : base(modelAccessor, sitemapService, logger)
        {
            // set class class for injected services
            _codelookupService = codelookupService ?? throw new ArgumentNullException(nameof(codelookupService));
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

        }

        #region Employees

        public IActionResult Index()
        {
            // set page title (required)
            PageTitle = Localization.EmployeeListTitle;
            // set description (optional)
            PageDescription = Localization.EmployeeListPageDescription;

            // call provate method to build employees view model.
            var viewModel = BuildEmployeesViewModel();

            return View(viewModel);
        }

        #endregion

        #region View Employee

        /// <summary>
        /// Details view to display readonly employee info.
        /// </summary>
        /// <param name="id">employee id</param>
        /// <returns>employee details view</returns>
        public IActionResult Details(int id)
        {
            // set page title (required)
            PageTitle = Localization.EmployeeViewTitle;
            // set description (optional)
            PageDescription = Localization.EmployeeViewPageDescription;

            // instantiate a view model class.
            var viewModel = new EmployeeViewModel();

            // call service to get the employee business object.
            var result = _employeeService.GetEmployee(id);

            // if successful...
            if (result.Success)
            {
                // ... map the employee business object to the employee view model.
                viewModel = _mapper.Map<EmployeeViewModel>(result.Value);
            }
            else
            {
                // check what type of business error you trapping and handle it.
                // in this case, we're only trapping record not found error... no other business errors
                // can happen on this call.
                if (result.ErrorType == ErrorType.RecordNotFound)
                {
                    // set error page title
                    PageTitle = Localization.GenericUntrappedErrorTitle;
                    PageDescription = null;

                    // return generic error page partial view with custom error message.
                    // by setting IsCustomError = true and providing a message, you can display your own message on the generic error page.
                    return View("Error", new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier, IsCustomError = true, ErrorMessage = result.ErrorMessages.FirstOrDefault() });
                }

            }

            // populate codelookup lists on the view model.
            PopulateCodelookups(viewModel);

            return View(viewModel);

        }

        #endregion

        #region Add Employee

        public IActionResult Add()
        {
            // set page title and description.
            PageTitle = Localization.EmployeeAddTitle;
            PageDescription = Localization.EmployeeAddPageDescription;

            var viewModel = new EmployeeViewModel();

            PopulateCodelookups(viewModel);

            return View(viewModel);
        }

        /// <summary>
        /// Employee add view
        /// </summary>
        /// <param name="viewModel">employee view model</param>
        /// <returns>edit employee view</returns>
        [HttpPost]
        public IActionResult Add(EmployeeViewModel viewModel)
        {
            // set page title and description.
            PageTitle = Localization.EmployeeAddTitle;
            PageDescription = Localization.EmployeeAddPageDescription;

            // check if modelstate is valid.  done in case browser has javascript disabled.
            if (ModelState.IsValid)
            {
                // map view model to business object.
                var employee = _mapper.Map<Employee>(viewModel);

                // call service method to add employee.  Service will know to add rather than edit because id = 0 on add.
                var result = _employeeService.AddOrEditEmployee(employee);

                if (result.Success)
                {
                    // set tempdata flag to indicate to edit view that we're coming from an add that was successfully executed.
                    // this way, on the edit view, we can show a success message.
                    TempData["EmployeeAdded"] = "Yes";

                    // redirect to the edit view so it can display employee that was just added.
                    return RedirectToAction("Edit", new { id = result.Value.Id });
                }
                else
                {
                    // in this case, we're only catching business logic errors.
                    // if any other exception happens (like an sql exception), it'll get thrown and caught by the generic error handler.
                    if (result.ErrorType == ErrorType.BusinessLogic)
                    {
                        foreach (var message in result.ErrorMessages) 
                        {
                            viewModel.AddErrorMessage(message);
                        };
                    }
                }
            }
            else
            {
                // add validation error to view model so they could be displayed
                // when this view is posted back.
                viewModel.ProcessModelStateErrorMessages(ModelState);
            }

            // populate codelookup lists on the viewmodel.
            PopulateCodelookups(viewModel);

            return View(viewModel);
        }

        #endregion

        #region Edit Employee

        public IActionResult Edit(int id)
        {
            // set page title and description.
            PageTitle = Localization.EmployeeEditTitle;
            PageDescription = Localization.EmployeeEditPageDescription;

            var viewModel = new EmployeeViewModel();

            var result = _employeeService.GetEmployee(id);

            if (result.Success)
            {
                viewModel = _mapper.Map<EmployeeViewModel>(result.Value);

                var comingFromAdd = TempData["EmployeeAdded"] as string;
                if (comingFromAdd == "Yes")
                {
                    // set success message.
                    viewModel.SetSuccessMessage(Localization.EmployeeAddSuccessMessage);
                }

            }
            else
            {
                // if record is not found, we want to display the generic error page since this not a business validation rule.
                if (result.ErrorType == ErrorType.RecordNotFound)
                {
                    // set error page title
                    PageTitle = Localization.GenericUntrappedErrorTitle;
                    PageDescription = null;

                    // return generic error page partial view with custom error message.
                    // by setting IsCustomError = true and providing a message, you can display your own message on the generic error page.
                    return View("Error", new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier, IsCustomError = true, ErrorMessage="Employee not found." });
                }

                foreach (var message in result.ErrorMessages)
                {
                    viewModel.AddErrorMessage(message);
                };
                
            }

            // populate code lookups in view model.
            PopulateCodelookups(viewModel);

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Edit(EmployeeViewModel viewModel)
        {
            // set page title and description.
            PageTitle = Localization.EmployeeEditTitle;
            PageDescription = Localization.EmployeeEditPageDescription;

            if (ModelState.IsValid)
            {
                // map view model to business object.
                var employee = _mapper.Map<Employee>(viewModel);

                // call service method to edit employee.  Service will know to edit rather than add because id will not be 0 on edit.
                var result = _employeeService.AddOrEditEmployee(employee);

                if (result.Success)
                {
                    // good practice to map the updated business object back to the view model even though in this case, nothing will have changed between the view model and the business object.
                    viewModel = _mapper.Map<EmployeeViewModel>(result.Value);

                    // set success message.
                    viewModel.SetSuccessMessage(Localization.EmployeeEditSuccessMessage); ;
                }
                else
                {
                    // in this case, we're only catching business logic errors.
                    // if any other exception happens (like an sql exception), it'll get thrown and caught by the generic error handler.
                    if (result.ErrorType == ErrorType.BusinessLogic)
                    {
                        foreach (var message in result.ErrorMessages)
                        {
                            viewModel.AddErrorMessage(message);
                        };
                    }
                }
            }
            else
            {
                // this is to handle client side validation if javascript is turned off on the browser.
                viewModel.ProcessModelStateErrorMessages(ModelState);
            }

            // populate code lookups in view model.
            PopulateCodelookups(viewModel);

            return View(viewModel);
        }
        #endregion

        #region Delete Employee

        public IActionResult Delete(int id)
        {

            // call service method to delete employee
            _employeeService.DeleteEmployee(id);

            // call private method to build view model which will refresh the employee list.
            var viewModel = BuildEmployeesViewModel();

            return PartialView("_EmployeeList", viewModel);
        }

#endregion

        #region Private methods

        private EmployeesViewModel BuildEmployeesViewModel()
        {
            // call service to get employees and map to view model.
            var viewModel = new EmployeesViewModel()
            {
                Employees = _mapper.Map<List<EmployeeItemViewModel>>(_employeeService.GetEmployees())
            };


            return viewModel;
        }

        private void PopulateCodelookups(EmployeeViewModel viewModel)
        {
            // we pass id's for when getting lookups in case we're in edit or view mode and the selected id happens to be logically deleted.
            // the repository methods will make sure to include inactive lookup in the list.
            viewModel.Provinces = CodelookupHelper.PopulateCodelookup(_codelookupService.GetProvinces(viewModel.ProvinceId), Localization.Codelookup_SelectProvince, true);
            viewModel.TermTypes = CodelookupHelper.PopulateCodelookup(_codelookupService.GetTermTypes(viewModel.TermTypeId));

        }

        #endregion
    }

}
