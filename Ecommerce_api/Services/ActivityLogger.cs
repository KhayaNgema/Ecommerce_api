using Ecommerce_api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Ecommerce_api.Data;
using Ecommerce_api.Interfaces;
using Ecommerce_api.Services;
using System;
using System.Threading.Tasks;

namespace Ecommerce.Services
{
    public class ActivityLogger : IActivityLogger
    {
        private readonly Ecommerce_apiDBContext _context;
        private readonly DeviceInfoService _deviceInfoService;
        private readonly UserManager<UserBaseModel> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ActivityLogger> _logger;

        public ActivityLogger(
            Ecommerce_apiDBContext context,
            DeviceInfoService deviceInfoService,
            UserManager<UserBaseModel> userManager,
            IHttpContextAccessor httpContextAccessor,
            ILogger<ActivityLogger> logger)
        {
            _context = context;
            _deviceInfoService = deviceInfoService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task Log(string activity, string userId)
        {
            var deviceInfo = await _deviceInfoService.GetDeviceInfo();

            var user = await _userManager.FindByIdAsync(userId);

            _context.Add(deviceInfo);
            await _context.SaveChangesAsync();

            var log = new ActivityLog
            {
                UserId = userId,
                Activity = activity,
                Timestamp = DateTime.Now,
                DeviceInfoId = deviceInfo.DeviceInfoId,
                UserBaseModel = user
            };

            _context.ActivityLogs.Add(log);
            await _context.SaveChangesAsync();
        }

    }
}
