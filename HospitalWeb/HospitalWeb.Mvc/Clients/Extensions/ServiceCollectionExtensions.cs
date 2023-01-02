using HospitalWeb.Mvc.Clients.Implementations;
using HospitalWeb.Mvc.Clients.Interfaces;
using HospitalWeb.Domain.Entities;
using HospitalWeb.Domain.Entities.Identity;
using HospitalWeb.Mvc.Models.ResourceModels;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddApi(this IServiceCollection services)
        {
            services.AddSingleton<ApiUnitOfWork>();
        }

        public static void AddHttpClientWithPolicies<TInterface, TImplementation>(this IServiceCollection services, string name)
            where TInterface : class
            where TImplementation : class, TInterface
        {
            services.AddHttpClient<TInterface, TImplementation>(name)
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5)))
                .AddTransientHttpErrorPolicy(p => p.CircuitBreakerAsync(2, TimeSpan.FromMinutes(1)))
                .AddPolicyHandler(request =>
                {
                    if (request.Method == HttpMethod.Get)
                    {
                        return Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(30));
                    }

                    return Policy.NoOpAsync<HttpResponseMessage>();
                });
        }

        public static void AddApiClients(this IServiceCollection services)
        {
            services.AddHttpClientWithPolicies<IApiClient<Address, AddressResourceModel, int>, AddressesApiClient>("Addresses");
            services.AddHttpClientWithPolicies<IApiClient<Admin, AdminResourceModel, string>, AdminsApiClient>("Admins");
            services.AddHttpClientWithPolicies<IApiClient<AppUser, AppUserResourceModel, string>, AppUsersApiClient>("AppUsers");
            services.AddHttpClientWithPolicies<IApiClient<Appointment, AppointmentResourceModel, int>, AppointmentsApiClient>("Appointments");
            services.AddHttpClientWithPolicies<IApiClient<Diagnosis, DiagnosisResourceModel, int>, DiagnosesApiClient>("Diagnoses");
            services.AddHttpClientWithPolicies<IApiClient<Doctor, DoctorResourceModel, string>, DoctorsApiClient>("Doctors");
            services.AddHttpClientWithPolicies<IApiClient<Hospital, HospitalResourceModel, int>, HospitalsApiClient>("Hospitals");
            services.AddHttpClientWithPolicies<IApiClient<Locality, LocalityResourceModel, int>, LocalitiesApiClient>("Localities");
            services.AddHttpClientWithPolicies<IApiClient<Meeting, MeetingResourceModel, int>, MeetingsApiClient>("Meetings");
            services.AddHttpClientWithPolicies<IApiClient<Notification, NotificationResourceModel, int>, NotificationsApiClient>("Notifications");
            services.AddHttpClientWithPolicies<IApiClient<Patient, PatientResourceModel, string>, PatientsApiClient>("Patients");
            services.AddHttpClientWithPolicies<IApiClient<Schedule, ScheduleResourceModel, int>, SchedulesApiClient>("Schedules");
            services.AddHttpClientWithPolicies<IApiClient<Specialty, SpecialtyResourceModel, int>, SpecialtiesApiClient>("Specialties");
        }
    }
}
