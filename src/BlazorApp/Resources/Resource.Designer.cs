﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BlazorApp.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("BlazorApp.Resources.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Name ist ein Mussfeld.
        /// </summary>
        public static string error_person_name {
            get {
                return ResourceManager.GetString("error.person.name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Geburtstag.
        /// </summary>
        public static string person_birthdate {
            get {
                return ResourceManager.GetString("person.birthdate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Geschlecht.
        /// </summary>
        public static string person_gender {
            get {
                return ResourceManager.GetString("person.gender", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Name.
        /// </summary>
        public static string person_name {
            get {
                return ResourceManager.GetString("person.name", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Wohnort.
        /// </summary>
        public static string person_residence {
            get {
                return ResourceManager.GetString("person.residence", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Einkommen (steuerbar).
        /// </summary>
        public static string person_taxableincome {
            get {
                return ResourceManager.GetString("person.taxableincome", resourceCulture);
            }
        }
    }
}
