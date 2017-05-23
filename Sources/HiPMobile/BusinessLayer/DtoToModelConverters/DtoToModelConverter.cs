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

using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Managers;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using Realms;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.DtoToModelConverters {
    /// <summary>
    /// Provides functionality for converting a dto of type <typeparamref name="TDtoObject"/> to 
    /// a model class of type <typeparamref name="TModelObject"/>
    /// </summary>
    /// <typeparam name="TModelObject"></typeparam>
    /// <typeparam name="TDtoObject"></typeparam>
    public abstract class DtoToModelConverter<TModelObject, TDtoObject> where TModelObject : RealmObject, IIdentifiable, new ()
    {
        /// <summary>
        /// Converts the given <paramref name="dto"/> to a new model object
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public TModelObject Convert (TDtoObject dto)
        {
            var modelObject = DbManager.CreateBusinessObject<TModelObject> ();

            Convert (dto, modelObject);

            return modelObject;
        }

        /// <summary>
        /// Converts the given <paramref name="dto"/> to the <paramref name="existingModelObject"/>
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="existingModelObject"></param>
        public abstract void Convert (TDtoObject dto, TModelObject existingModelObject);

    }
}