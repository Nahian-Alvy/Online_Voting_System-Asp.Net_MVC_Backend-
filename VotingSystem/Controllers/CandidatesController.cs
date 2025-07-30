using System;
using System.Collections.Generic;
using System.Web.Http;
using VotingSystem.Business.DTOs;
using VotingSystem.Business.Services;

namespace VotingSystem.API.Controllers
{
    [RoutePrefix("api/candidates")]
    public class CandidatesController : ApiController
    {
        private readonly ICandidateService _candidateService;

        public CandidatesController(ICandidateService candidateService)
        {
            _candidateService = candidateService ?? throw new ArgumentNullException(nameof(candidateService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll(int? electionId = null)
        {
            try
            {
                if (electionId.HasValue)
                {
                    var candidates = _candidateService.GetCandidatesByElection(electionId.Value) ??
                                     new List<CandidateDto>();
                    return Ok(candidates);
                }
                else
                {
                    var candidates = _candidateService.GetAllCandidates() ?? new List<CandidateDto>();
                    return Ok(candidates);
                }
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
                var candidate = _candidateService.GetCandidateById(id);
                if (candidate == null)
                    return NotFound();

                return Ok(candidate);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(CandidateCreateDto candidateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Add default values if null to avoid null reference exceptions
                if (candidateDto == null)
                {
                    candidateDto = new CandidateCreateDto
                    {
                        Name = "Default Candidate",
                        ElectionId = 1,
                        Platform = "Default Platform",
                        PhotoUrl = "/images/default.jpg"
                    };
                }

                // Set default values for null properties
                candidateDto.Name = candidateDto.Name ?? "Unnamed Candidate";
                candidateDto.Platform = candidateDto.Platform ?? "No platform provided";
                candidateDto.PhotoUrl = candidateDto.PhotoUrl ?? "/images/default.jpg";

                var candidate = _candidateService.CreateCandidate(candidateDto);
                return CreatedAtRoute("DefaultApi", new { id = candidate.CandidateId }, candidate);
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
        public IHttpActionResult Update(int id, CandidateCreateDto candidateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Default values if null
                if (candidateDto == null)
                {
                    candidateDto = new CandidateCreateDto
                    {
                        Name = "Updated Candidate",
                        ElectionId = 1,
                        Platform = "Updated Platform",
                        PhotoUrl = "/images/default.jpg"
                    };
                }

                // Set default values for null properties
                candidateDto.Name = candidateDto.Name ?? "Updated Candidate";
                candidateDto.Platform = candidateDto.Platform ?? "Updated platform";
                candidateDto.PhotoUrl = candidateDto.PhotoUrl ?? "/images/default.jpg";

                var candidate = _candidateService.UpdateCandidate(id, candidateDto);
                if (candidate == null)
                    return NotFound();

                return Ok(candidate);
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
                var result = _candidateService.DeleteCandidate(id);
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
        [Route("election/{electionId:int}")]
        public IHttpActionResult GetByElection(int electionId)
        {
            try
            {
                var candidates = _candidateService.GetCandidatesByElection(electionId) ??
                                 new List<CandidateDto>();
                return Ok(candidates);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}