﻿using System;
using System.Collections.Generic;
using System.Text;
using daemon_console.Models;
namespace daemon_console.Models
{
    internal class ApiCaller
    {
       
        public static string GetSite(string SiteId = "")
        {
            return $"sites/{SiteId}";
        }

        public static string GetDriveBySite(string SiteId)
        {
            return $"sites/{SiteId}/drive";
        }

        public static string GetFilesByDrive(string DriveId, bool RootOnnly = false)
        {
            if (RootOnnly)
                return $"/drives/{DriveId}/root/children";
            else
            {
                return $"";
            }
        }
    }
}