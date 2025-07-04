﻿using System;
using System.Data.Entity.Core.Objects;
using Wfm.Core;

namespace Wfm.Data
{
    public static class Extensions
    {
        /// <summary>
        /// Get unproxied entity type
        /// </summary>
        /// <remarks> If your Entity Framework context is proxy-enabled, 
        /// the runtime will create a proxy instance of your entities, 
        /// i.e. a dynamically generated class which inherits from your entity class 
        /// and overrides its virtual properties by inserting specific code useful for example 
        /// for tracking changes and lazy loading.
        /// </remarks>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Type GetUnproxiedEntityType(this BaseEntity entity)
        {
            var entityType = ObjectContext.GetObjectType(entity.GetType());
            return entityType;
        }
    }
}
