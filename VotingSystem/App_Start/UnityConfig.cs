using System.Web.Http;
using Unity;
using Unity.WebApi;
using Unity.AspNet.WebApi;
using VotingSystem.Business.Services;
using VotingSystem.Data;
using VotingSystem.Data.Repositories;
using Unity.AspNet.Mvc;
using UnityDependencyResolver = Unity.WebApi.UnityDependencyResolver;

namespace VotingSystem.API
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // Register database context
            container.RegisterType<VotingSystemContext>(new PerRequestLifetimeManager());

            // Register repositories
            container.RegisterType<IUnitOfWork, UnitOfWork>();

            // Register services
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IElectionService, ElectionService>();
            container.RegisterType<ICandidateService, CandidateService>();
            container.RegisterType<IVotingService, VotingService>();
            container.RegisterType<IVoterVerificationService, VoterVerificationService>();
            container.RegisterType<IReportingService, ReportingService>();
            container.RegisterType<ICandidateService, CandidateService>();
            // Register services
            container.RegisterType<IVoterVerificationService, VoterVerificationService>();
            container.RegisterType<IVotingService, VotingService>();
            container.RegisterType<IReportingService, ReportingService>();
            // In UnityConfig.cs or similar configuration file
            container.RegisterType<IUnitOfWork, UnitOfWork>(new PerRequestLifetimeManager());
            container.RegisterType<VotingSystemContext, VotingSystemContext>();

            // Set the dependency resolver for Web API
            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);
        }
    }
}