﻿using System;

/**
 * author:qixiao
 * create:2017-8-7 10:19:35
 * */
namespace QX_Frame.Helper_DG.Bantina
{
    /// <summary>
    /// Self Define Attribute , Support Properties/Class Support Extend Inherit
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// Real ColumnName : if not declare , use property name.
        /// </summary>
        public string ColumnName { get; set; }
    }
}