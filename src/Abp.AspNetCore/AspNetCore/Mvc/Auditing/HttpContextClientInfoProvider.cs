﻿using Abp.Auditing;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;

namespace Abp.AspNetCore.Mvc.Auditing
{
    public class HttpContextClientInfoProvider : IClientInfoProvider
    {
        public string BrowserInfo => GetBrowserInfo();

        public string ClientIpAddress => GetClientIpAddress();

        public string ComputerName => GetComputerName();

        public ILogger Logger { get; set; }

        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Creates a new <see cref="HttpContextClientInfoProvider"/>.
        /// </summary>
        public HttpContextClientInfoProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            Logger = NullLogger.Instance;
        }

        protected virtual string GetBrowserInfo()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.Request?.Headers?["User-Agent"];
        }

        protected virtual string GetClientIpAddress()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                return httpContext?.Connection?.RemoteIpAddress?.ToString();

            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString());
            }

            return null;
        }

        protected virtual string GetComputerName()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var remoteIpAddress = httpContext?.Connection?.RemoteIpAddress;

                if (remoteIpAddress is null) return null;

                return Dns.GetHostEntry(remoteIpAddress).HostName;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString());
            }

            return null;
        }
    }
}
