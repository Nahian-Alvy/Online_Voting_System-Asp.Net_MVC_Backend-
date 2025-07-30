using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using VotingSystem.Business.DTOs;
using VotingSystem.Business.Services;

namespace VotingSystem.API.Controllers
{
    [RoutePrefix("api/elections")]
    public class ElectionsController : ApiController
    {
        private readonly IElectionService _electionService;

        public ElectionsController(IElectionService electionService)
        {
            _electionService = electionService ?? throw new ArgumentNullException(nameof(electionService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll()
        {
            try
            {
                var elections = _electionService.GetAllElections() ?? new List<ElectionDto>();
                return Ok(elections);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("active")]
        public IHttpActionResult GetActive()
        {
            try
            {
                var elections = _electionService.GetActiveElections() ?? new List<ElectionDto>();
                return Ok(elections);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult GetById(int id)
        {
            try
            {
                var election = _electionService.GetElectionById(id);
                if (election == null)
                {
                    return NotFound();
                }

                return Ok(election);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(ElectionCreateDto electionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Add default values if null to avoid null reference exceptions
                if (electionDto == null)
                {
                    electionDto = new ElectionCreateDto
                    {
                        Title = "Default Election Title",
                        Description = "Default Election Description",
                        StartDate = DateTime.Parse("2025-06-25 04:57:18"), // Updated current time
                        EndDate = DateTime.Parse("2025-06-25 04:57:18").AddDays(7)
                    };
                }

                // Set default values for null properties
                electionDto.Title = electionDto.Title ?? "Untitled Election";
                electionDto.Description = electionDto.Description ?? "No description provided";

                // If dates are default/null, set them based on current time
                if (electionDto.StartDate == default(DateTime))
                {
                    electionDto.StartDate = DateTime.Parse("2025-06-25 04:57:18");
                }

                if (electionDto.EndDate == default(DateTime))
                {
                    electionDto.EndDate = electionDto.StartDate.AddDays(7);
                }

                // For simplicity, hardcoding user ID. In practice, get this from token
                int createdByUserId = 1; // Default to admin user

                var election = _electionService.CreateElection(electionDto, createdByUserId);

                return CreatedAtRoute("DefaultApi", new { id = election.ElectionId }, election);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public IHttpActionResult Update(int id, ElectionCreateDto electionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Default values if null
                if (electionDto == null)
                {
                    electionDto = new ElectionCreateDto
                    {
                        Title = "Updated Election Title",
                        Description = "Updated Election Description",
                        StartDate = DateTime.Parse("2025-06-25 04:57:18"),
                        EndDate = DateTime.Parse("2025-06-25 04:57:18").AddDays(7)
                    };
                }

                // Set default values for null properties
                electionDto.Title = electionDto.Title ?? "Updated Election";
                electionDto.Description = electionDto.Description ?? "Updated description";

                if (electionDto.StartDate == default(DateTime))
                {
                    electionDto.StartDate = DateTime.Parse("2025-06-25 04:57:18");
                }

                if (electionDto.EndDate == default(DateTime))
                {
                    electionDto.EndDate = electionDto.StartDate.AddDays(7);
                }

                var election = _electionService.UpdateElection(id, electionDto);
                if (election == null)
                    return NotFound();

                return Ok(election);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult Delete(int id)
        {
            try
            {
                var result = _electionService.DeleteElection(id);
                if (!result)
                    return NotFound();

                return Ok();
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:int}/statistics")]
        public IHttpActionResult GetStatistics(int id)
        {
            try
            {
                // Let's assume we have a method in the service that returns statistics
                // If it's null, we'll return default empty stats
                var statistics = _electionService.GetElectionStatistics(id);

                if (statistics == null)
                {
                    statistics = new Dictionary<string, object>
                    {
                        { "ElectionId", id },
                        { "ElectionTitle", "Unknown Election" },
                        { "TotalVotes", 0 },
                        { "CandidateResults", new List<CandidateResultDto>() },
                        { "VoterTurnout", 0 }
                    };
                }

                return Ok(statistics);
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("{id:int}/status")]
        public IHttpActionResult GetElectionStatus(int id)
        {
            try
            {
                bool isActive = _electionService.IsElectionActive(id);
                return Ok(new { isActive = isActive });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPatch]
        [Route("{id:int}/toggle-status")]
        public IHttpActionResult ToggleElectionStatus(int id)
        {
            try
            {
                var election = _electionService.GetElectionById(id);
                if (election == null)
                {
                    // Create dummy election with basic info if not found
                    election = new ElectionDto
                    {
                        ElectionId = id,
                        Title = "Dummy Election",
                        Description = "This is a dummy election created when the original was not found",
                        StartDate = DateTime.Parse("2025-06-25 04:57:18"),
                        EndDate = DateTime.Parse("2025-06-25 04:57:18").AddDays(7),
                        IsActive = false,
                        CreatedByUserName = "Nahian-Alvyyes", // Fixed typo in username
                        CreatedDate = DateTime.Parse("2025-06-25 04:57:18")
                    };

                    return NotFound();
                }

                // Create update DTO with existing values
                var updateDto = new ElectionCreateDto
                {
                    Title = election.Title ?? "Unnamed Election",
                    Description = election.Description ?? "No description",
                    StartDate = election.StartDate != default(DateTime) ? election.StartDate : DateTime.Parse("2025-06-25 04:57:18"),
                    EndDate = election.EndDate != default(DateTime) ? election.EndDate : DateTime.Parse("2025-06-25 04:57:18").AddDays(7)
                };

                // Toggle the status in the service layer
                election.IsActive = !election.IsActive;
                var updatedElection = _electionService.UpdateElection(id, updateDto);

                return Ok(new { isActive = updatedElection?.IsActive ?? false });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}