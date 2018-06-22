// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using JetBrains.Annotations;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer;
using System;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters
{
    /// <summary>
    /// Provides functionality for converting a dto of type <typeparamref name="TDtoObject"/> to 
    /// a model class of type <typeparamref name="TModelObject"/>
    /// </summary>
    /// <typeparam name="TModelObject"></typeparam>
    /// <typeparam name="TDtoObject"></typeparam>
    public abstract class DtoToModelConverter<TModelObject, TDtoObject> where TModelObject : class, IIdentifiable
    {
        /// <summary>
        /// Converts the given <paramref name="dto"/> to a new model object
        /// </summary>
        public TModelObject Convert(TDtoObject dto)
        {
            var modelObject = CreateModelInstance(dto);
            Convert(dto, modelObject);
            return modelObject;
        }

        /// <summary>
        /// Converts the given <paramref name="dto"/> to a new model object, deleting the entity first if it already exists.
        /// </summary>
        public TModelObject ConvertReplacingExisting(TDtoObject dto, [NotNull] string id, ITransactionDataAccess dataAccess)
        {
            var modelObject = dataAccess.GetItem<TModelObject>(id) ?? CreateModelInstance(dto);
            modelObject.Id = id ?? throw new ArgumentNullException(nameof(id));
            Convert(dto, modelObject);
            return modelObject;
        }

        /// <summary>
        /// Converts the given <paramref name="dto"/> to the <paramref name="existingModelObject"/>
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="existingModelObject"></param>
        public abstract void Convert(TDtoObject dto, TModelObject existingModelObject);

        /// <summary>
        /// Creates an empty instance of <typeparamref name="TModelObject"/>.
        /// By default, this tries to use the parameterless constructor of <typeparamref name="TModelObject"/>.
        /// This method only needs to be overridden if such a constructor is not available, e.g. if
        /// <typeparamref name="TModelObject"/> is abstract and thus can't be constructed.
        /// </summary>
        /// <param name="dto"></param>
        protected virtual TModelObject CreateModelInstance(TDtoObject dto)
        {
            try
            {
                return Activator.CreateInstance<TModelObject>();
            }
            catch (MissingMethodException e)
            {
                throw new InvalidOperationException(
                    $"'{GetType().Name}' failed to create an instance of '{typeof(TModelObject).Name}' because the type " +
                    $"does not have a parameterless constructor or is abstract. Consider adding a parameterless constructor " +
                    $"or overriding '{nameof(CreateModelInstance)}'.", e);
            }
        }
    }
}