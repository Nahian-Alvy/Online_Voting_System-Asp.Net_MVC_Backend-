using System;
using System.Collections.Generic;
using System.Web.Http;
using VotingSystem.Business.DTOs;
using VotingSystem.Business.Services;

namespace VotingSystem.API.Controllers
{
    [RoutePrefix("api/voter-verifications")]
    public class VoterVerificationsController : ApiController
    {
        private readonly IVoterVerificationService _verificationService;

        public VoterVerificationsController(IVoterVerificationService verificationService)
        {
            _verificationService = verificationService ?? throw new ArgumentNullException(nameof(verificationService));
        }

        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAll(int? electionId = null, int? userId = null)
        {
            try
            {
                IEnumerable<VoterVerificationDto> verifications;

                if (electionId.HasValue && userId.HasValue)
                {
                    verifications = (IEnumerable<VoterVerificationDto>)_verificationService.GetVerificationByElectionAndUser(electionId.Value, userId.Value);
                }
                else if (electionId.HasValue)
                {
                    verifications = _verificationService.GetVerificationsByElection(electionId.Value);
                }
                else if (userId.HasValue)
                {
                    verifications = _verificationService.GetVerificationsByUser(userId.Value);
                }
                else
                {
                    verifications = _verificationService.GetAllVerifications();
                }

                return Ok(verifications ?? new List<VoterVerificationDto>());
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
                var verification = _verificationService.GetVerificationById(id);
                if (verification == null)
                    return NotFound();

                return Ok(verification);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Create(VoterVerificationCreateDto verificationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (verificationDto == null)
                {
                    return BadRequest("Verification data is required");
                }

                var verification = _verificationService.CreateVerification(verificationDto);

                // Return the created verification but hide the actual code in the response
                // for security reasons
                var result = new
                {
                    verification.VerificationId,
                    verification.UserId,
                    verification.ElectionId,
                    verification.IsUsed,
                    verification.ExpiryDate,
                    // Only show verification code in initial creation response
                    VerificationCode = verification.VerificationCode
                };

                return CreatedAtRoute("DefaultApi", new { id = verification.VerificationId }, result);
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
        [Route("{id:int}/mark-used")]
        public IHttpActionResult MarkAsUsed(int id)
        {
            try
            {
                var result = _verificationService.MarkVerificationAsUsed(id);
                if (!result)
                    return NotFound();

                return Ok(new { success = true, message = "Verification marked as used" });
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
        [Route("validate")]
        public IHttpActionResult ValidateCode(string code, int electionId)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                    return BadRequest("Verification code is required");

                var isValid = _verificationService.ValidateVerificationCode(code, electionId);
                return Ok(new { isValid = isValid });
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