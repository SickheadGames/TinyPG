// Copyright 2008 - 2010 Herre Kuijpers - <herre.kuijpers@gmail.com>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace TinyPG.Controls
{
    public class AssemblyInfo
    {
        static string companyName = string.Empty;
        /// Get the name of the system provider name from the assembly ///
        public static string CompanyName
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                    {
                        companyName = ((AssemblyCompanyAttribute)customAttributes[0]).Company;
                    }
                    if (string.IsNullOrEmpty(companyName))
                    {
                        companyName = string.Empty;
                    }
                }
                return companyName;
            }
        }
        static string productVersion = string.Empty; ///
        /// Get System version from the assembly ///
        public static string ProductVersion
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyVersionAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                    {
                        productVersion = ((AssemblyVersionAttribute)customAttributes[0]).Version;
                    }
                    if (string.IsNullOrEmpty(productVersion))
                    {
                        productVersion = string.Empty;
                    }
                }
                return productVersion;
            }
        }
        static string productName = string.Empty; ///
        /// Get the name of the System from the assembly ///
        public static string ProductName
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                    {
                        productName = ((AssemblyProductAttribute)customAttributes[0]).Product;
                    }
                    if (string.IsNullOrEmpty(productName))
                    {
                        productName = string.Empty;
                    }
                }
                return productName;
            }
        }
        static string copyRightsDetail = string.Empty; ///
        /// Get the copyRights details from the assembly ///
        public static string CopyRightsDetail
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                    {
                        copyRightsDetail = ((AssemblyCopyrightAttribute)customAttributes[0]).Copyright;
                    }
                    if (string.IsNullOrEmpty(copyRightsDetail))
                    {
                        copyRightsDetail = string.Empty;
                    }
                }
                return copyRightsDetail;
            }
        }
        static string productTitle = string.Empty; ///
        /// Get the Product tile from the assembly ///
        public static string ProductTitle
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                    {
                        productTitle = ((AssemblyTitleAttribute)customAttributes[0]).Title;
                    }
                    if (string.IsNullOrEmpty(productTitle))
                    {
                        productTitle = string.Empty;
                    }
                }
                return productTitle;
            }
        }
        static string productDescription = string.Empty; ///
        /// Get the description of the product from the assembly ///
        public static string ProductDescription
        {
            get
            {
                Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
                if (assembly != null)
                {
                    object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                    if ((customAttributes != null) && (customAttributes.Length > 0))
                    {
                        productDescription = ((AssemblyDescriptionAttribute)customAttributes[0]).Description;
                    }
                    if (string.IsNullOrEmpty(productDescription))
                    {
                        productDescription = string.Empty;
                    }
                }
                return productDescription;
            }
        }
    }
}
