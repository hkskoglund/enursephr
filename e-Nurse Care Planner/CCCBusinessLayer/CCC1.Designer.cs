//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.1433
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: global::System.Data.Objects.DataClasses.EdmSchemaAttribute()]
[assembly: global::System.Data.Objects.DataClasses.EdmRelationshipAttribute("CCC.Businesslayer", "FK_Nursing_Diagnosis_Care_component", "Care_component", global::System.Data.Metadata.Edm.RelationshipMultiplicity.One, typeof(CCC.Businesslayer.Care_component), "Nursing_Diagnosis", global::System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(CCC.Businesslayer.Nursing_Diagnosis))]
[assembly: global::System.Data.Objects.DataClasses.EdmRelationshipAttribute("CCC.Businesslayer", "FK_Nursing_Intervention_Care_component", "Care_component", global::System.Data.Metadata.Edm.RelationshipMultiplicity.One, typeof(CCC.Businesslayer.Care_component), "Nursing_Intervention", global::System.Data.Metadata.Edm.RelationshipMultiplicity.Many, typeof(CCC.Businesslayer.Nursing_Intervention))]

// Original file name:
// Generation date: 12.01.2008 13:51:10
namespace CCC.Businesslayer
{
    
    /// <summary>
    /// There are no comments for CCCFrameworkEntities in the schema.
    /// </summary>
    public partial class CCCFrameworkEntities : global::System.Data.Objects.ObjectContext
    {
        /// <summary>
        /// Initializes a new CCCFrameworkEntities object using the connection string found in the 'CCCFrameworkEntities' section of the application configuration file.
        /// </summary>
        public CCCFrameworkEntities() : 
                base("name=CCCFrameworkEntities", "CCCFrameworkEntities")
        {
        }
        /// <summary>
        /// Initialize a new CCCFrameworkEntities object.
        /// </summary>
        public CCCFrameworkEntities(string connectionString) : 
                base(connectionString, "CCCFrameworkEntities")
        {
        }
        /// <summary>
        /// Initialize a new CCCFrameworkEntities object.
        /// </summary>
        public CCCFrameworkEntities(global::System.Data.EntityClient.EntityConnection connection) : 
                base(connection, "CCCFrameworkEntities")
        {
        }
        /// <summary>
        /// There are no comments for Care_componentSet in the schema.
        /// </summary>
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public global::System.Data.Objects.ObjectQuery<Care_component> Care_componentSet
        {
            get
            {
                if ((this._Care_componentSet == null))
                {
                    this._Care_componentSet = base.CreateQuery<Care_component>("[Care_componentSet]");
                }
                return this._Care_componentSet;
            }
        }
        private global::System.Data.Objects.ObjectQuery<Care_component> _Care_componentSet;
        /// <summary>
        /// There are no comments for CarePatternSet in the schema.
        /// </summary>
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public global::System.Data.Objects.ObjectQuery<CarePattern> CarePatternSet
        {
            get
            {
                if ((this._CarePatternSet == null))
                {
                    this._CarePatternSet = base.CreateQuery<CarePattern>("[CarePatternSet]");
                }
                return this._CarePatternSet;
            }
        }
        private global::System.Data.Objects.ObjectQuery<CarePattern> _CarePatternSet;
        /// <summary>
        /// There are no comments for Nursing_DiagnosisSet in the schema.
        /// </summary>
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public global::System.Data.Objects.ObjectQuery<Nursing_Diagnosis> Nursing_DiagnosisSet
        {
            get
            {
                if ((this._Nursing_DiagnosisSet == null))
                {
                    this._Nursing_DiagnosisSet = base.CreateQuery<Nursing_Diagnosis>("[Nursing_DiagnosisSet]");
                }
                return this._Nursing_DiagnosisSet;
            }
        }
        private global::System.Data.Objects.ObjectQuery<Nursing_Diagnosis> _Nursing_DiagnosisSet;
        /// <summary>
        /// There are no comments for Nursing_InterventionSet in the schema.
        /// </summary>
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public global::System.Data.Objects.ObjectQuery<Nursing_Intervention> Nursing_InterventionSet
        {
            get
            {
                if ((this._Nursing_InterventionSet == null))
                {
                    this._Nursing_InterventionSet = base.CreateQuery<Nursing_Intervention>("[Nursing_InterventionSet]");
                }
                return this._Nursing_InterventionSet;
            }
        }
        private global::System.Data.Objects.ObjectQuery<Nursing_Intervention> _Nursing_InterventionSet;
        /// <summary>
        /// There are no comments for Care_componentSet in the schema.
        /// </summary>
        public void AddToCare_componentSet(Care_component care_component)
        {
            base.AddObject("Care_componentSet", care_component);
        }
        /// <summary>
        /// There are no comments for CarePatternSet in the schema.
        /// </summary>
        public void AddToCarePatternSet(CarePattern carePattern)
        {
            base.AddObject("CarePatternSet", carePattern);
        }
        /// <summary>
        /// There are no comments for Nursing_DiagnosisSet in the schema.
        /// </summary>
        public void AddToNursing_DiagnosisSet(Nursing_Diagnosis nursing_Diagnosis)
        {
            base.AddObject("Nursing_DiagnosisSet", nursing_Diagnosis);
        }
        /// <summary>
        /// There are no comments for Nursing_InterventionSet in the schema.
        /// </summary>
        public void AddToNursing_InterventionSet(Nursing_Intervention nursing_Intervention)
        {
            base.AddObject("Nursing_InterventionSet", nursing_Intervention);
        }
    }
    /// <summary>
    /// There are no comments for CCC.Businesslayer.Care_component in the schema.
    /// </summary>
    /// <KeyProperties>
    /// Language_Name
    /// Code
    /// </KeyProperties>
    [global::System.Data.Objects.DataClasses.EdmEntityTypeAttribute(NamespaceName="CCC.Businesslayer", Name="Care_component")]
    [global::System.Runtime.Serialization.DataContractAttribute()]
    [global::System.Serializable()]
    public partial class Care_component : global::System.Data.Objects.DataClasses.EntityObject
    {
        /// <summary>
        /// Create a new Care_component object.
        /// </summary>
        /// <param name="language_Name">Initial value of Language_Name.</param>
        /// <param name="code">Initial value of Code.</param>
        /// <param name="pattern">Initial value of Pattern.</param>
        public static Care_component CreateCare_component(string language_Name, string code, string pattern)
        {
            Care_component care_component = new Care_component();
            care_component.Language_Name = language_Name;
            care_component.Code = code;
            care_component.Pattern = pattern;
            return care_component;
        }
        /// <summary>
        /// There are no comments for Property Language_Name in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public string Language_Name
        {
            get
            {
                return this._Language_Name;
            }
            set
            {
                this.OnLanguage_NameChanging(value);
                this.ReportPropertyChanging("Language_Name");
                this._Language_Name = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, false, 25);
                this.ReportPropertyChanged("Language_Name");
                this.OnLanguage_NameChanged();
            }
        }
        private string _Language_Name;
        partial void OnLanguage_NameChanging(string value);
        partial void OnLanguage_NameChanged();
        /// <summary>
        /// There are no comments for Property Code in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public string Code
        {
            get
            {
                return this._Code;
            }
            set
            {
                this.OnCodeChanging(value);
                this.ReportPropertyChanging("Code");
                this._Code = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, false, 1);
                this.ReportPropertyChanged("Code");
                this.OnCodeChanged();
            }
        }
        private string _Code;
        partial void OnCodeChanging(string value);
        partial void OnCodeChanged();
        /// <summary>
        /// There are no comments for Property Pattern in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public string Pattern
        {
            get
            {
                return this._Pattern;
            }
            set
            {
                this.OnPatternChanging(value);
                this.ReportPropertyChanging("Pattern");
                this._Pattern = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, false, 50);
                this.ReportPropertyChanged("Pattern");
                this.OnPatternChanged();
            }
        }
        private string _Pattern;
        partial void OnPatternChanging(string value);
        partial void OnPatternChanged();
        /// <summary>
        /// There are no comments for Property Component in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute()]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public string Component
        {
            get
            {
                return this._Component;
            }
            set
            {
                this.OnComponentChanging(value);
                this.ReportPropertyChanging("Component");
                this._Component = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, true, 50);
                this.ReportPropertyChanged("Component");
                this.OnComponentChanged();
            }
        }
        private string _Component;
        partial void OnComponentChanging(string value);
        partial void OnComponentChanged();
        /// <summary>
        /// There are no comments for Property Definition in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute()]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public string Definition
        {
            get
            {
                return this._Definition;
            }
            set
            {
                this.OnDefinitionChanging(value);
                this.ReportPropertyChanging("Definition");
                this._Definition = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, true, 255);
                this.ReportPropertyChanged("Definition");
                this.OnDefinitionChanged();
            }
        }
        private string _Definition;
        partial void OnDefinitionChanging(string value);
        partial void OnDefinitionChanged();
        /// <summary>
        /// There are no comments for Nursing_Diagnosis in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmRelationshipNavigationPropertyAttribute("CCC.Businesslayer", "FK_Nursing_Diagnosis_Care_component", "Nursing_Diagnosis")]
        [global::System.Xml.Serialization.XmlIgnoreAttribute()]
        [global::System.Xml.Serialization.SoapIgnoreAttribute()]
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public global::System.Data.Objects.DataClasses.EntityCollection<Nursing_Diagnosis> Nursing_Diagnosis
        {
            get
            {
                return ((global::System.Data.Objects.DataClasses.IEntityWithRelationships)(this)).RelationshipManager.GetRelatedCollection<Nursing_Diagnosis>("CCC.Businesslayer.FK_Nursing_Diagnosis_Care_component", "Nursing_Diagnosis");
            }
        }
        /// <summary>
        /// There are no comments for Nursing_Intervention in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmRelationshipNavigationPropertyAttribute("CCC.Businesslayer", "FK_Nursing_Intervention_Care_component", "Nursing_Intervention")]
        [global::System.Xml.Serialization.XmlIgnoreAttribute()]
        [global::System.Xml.Serialization.SoapIgnoreAttribute()]
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public global::System.Data.Objects.DataClasses.EntityCollection<Nursing_Intervention> Nursing_Intervention
        {
            get
            {
                return ((global::System.Data.Objects.DataClasses.IEntityWithRelationships)(this)).RelationshipManager.GetRelatedCollection<Nursing_Intervention>("CCC.Businesslayer.FK_Nursing_Intervention_Care_component", "Nursing_Intervention");
            }
        }
    }
    /// <summary>
    /// There are no comments for CCC.Businesslayer.CarePattern in the schema.
    /// </summary>
    /// <KeyProperties>
    /// Language_Name
    /// Pattern
    /// </KeyProperties>
    [global::System.Data.Objects.DataClasses.EdmEntityTypeAttribute(NamespaceName="CCC.Businesslayer", Name="CarePattern")]
    [global::System.Runtime.Serialization.DataContractAttribute()]
    [global::System.Serializable()]
    public partial class CarePattern : global::System.Data.Objects.DataClasses.EntityObject
    {
        /// <summary>
        /// Create a new CarePattern object.
        /// </summary>
        /// <param name="language_Name">Initial value of Language_Name.</param>
        /// <param name="pattern">Initial value of Pattern.</param>
        public static CarePattern CreateCarePattern(string language_Name, string pattern)
        {
            CarePattern carePattern = new CarePattern();
            carePattern.Language_Name = language_Name;
            carePattern.Pattern = pattern;
            return carePattern;
        }
        /// <summary>
        /// There are no comments for Property Language_Name in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public string Language_Name
        {
            get
            {
                return this._Language_Name;
            }
            set
            {
                this.OnLanguage_NameChanging(value);
                this.ReportPropertyChanging("Language_Name");
                this._Language_Name = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, false, 25);
                this.ReportPropertyChanged("Language_Name");
                this.OnLanguage_NameChanged();
            }
        }
        private string _Language_Name;
        partial void OnLanguage_NameChanging(string value);
        partial void OnLanguage_NameChanged();
        /// <summary>
        /// There are no comments for Property Pattern in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public string Pattern
        {
            get
            {
                return this._Pattern;
            }
            set
            {
                this.OnPatternChanging(value);
                this.ReportPropertyChanging("Pattern");
                this._Pattern = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, false, 50);
                this.ReportPropertyChanged("Pattern");
                this.OnPatternChanged();
            }
        }
        private string _Pattern;
        partial void OnPatternChanging(string value);
        partial void OnPatternChanged();
        /// <summary>
        /// There are no comments for Property Definition in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute()]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public string Definition
        {
            get
            {
                return this._Definition;
            }
            set
            {
                this.OnDefinitionChanging(value);
                this.ReportPropertyChanging("Definition");
                this._Definition = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, true, 255);
                this.ReportPropertyChanged("Definition");
                this.OnDefinitionChanged();
            }
        }
        private string _Definition;
        partial void OnDefinitionChanging(string value);
        partial void OnDefinitionChanged();
    }
    /// <summary>
    /// There are no comments for CCC.Businesslayer.Nursing_Diagnosis in the schema.
    /// </summary>
    /// <KeyProperties>
    /// Language_Name
    /// ComponentCode
    /// MajorCode
    /// Concept
    /// </KeyProperties>
    [global::System.Data.Objects.DataClasses.EdmEntityTypeAttribute(NamespaceName="CCC.Businesslayer", Name="Nursing_Diagnosis")]
    [global::System.Runtime.Serialization.DataContractAttribute()]
    [global::System.Serializable()]
    public partial class Nursing_Diagnosis : global::System.Data.Objects.DataClasses.EntityObject
    {
        /// <summary>
        /// Create a new Nursing_Diagnosis object.
        /// </summary>
        /// <param name="language_Name">Initial value of Language_Name.</param>
        /// <param name="diagnosisID">Initial value of DiagnosisID.</param>
        /// <param name="componentCode">Initial value of ComponentCode.</param>
        /// <param name="majorCode">Initial value of MajorCode.</param>
        /// <param name="concept">Initial value of Concept.</param>
        public static Nursing_Diagnosis CreateNursing_Diagnosis(string language_Name, int diagnosisID, string componentCode, decimal majorCode, string concept)
        {
            Nursing_Diagnosis nursing_Diagnosis = new Nursing_Diagnosis();
            nursing_Diagnosis.Language_Name = language_Name;
            nursing_Diagnosis.DiagnosisID = diagnosisID;
            nursing_Diagnosis.ComponentCode = componentCode;
            nursing_Diagnosis.MajorCode = majorCode;
            nursing_Diagnosis.Concept = concept;
            return nursing_Diagnosis;
        }
        /// <summary>
        /// There are no comments for Property Language_Name in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public string Language_Name
        {
            get
            {
                return this._Language_Name;
            }
            set
            {
                this.OnLanguage_NameChanging(value);
                this.ReportPropertyChanging("Language_Name");
                this._Language_Name = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, false, 25);
                this.ReportPropertyChanged("Language_Name");
                this.OnLanguage_NameChanged();
            }
        }
        private string _Language_Name;
        partial void OnLanguage_NameChanging(string value);
        partial void OnLanguage_NameChanged();
        /// <summary>
        /// There are no comments for Property DiagnosisID in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public int DiagnosisID
        {
            get
            {
                return this._DiagnosisID;
            }
            set
            {
                this.OnDiagnosisIDChanging(value);
                this.ReportPropertyChanging("DiagnosisID");
                this._DiagnosisID = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("DiagnosisID");
                this.OnDiagnosisIDChanged();
            }
        }
        private int _DiagnosisID;
        partial void OnDiagnosisIDChanging(int value);
        partial void OnDiagnosisIDChanged();
        /// <summary>
        /// There are no comments for Property ComponentCode in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public string ComponentCode
        {
            get
            {
                return this._ComponentCode;
            }
            set
            {
                this.OnComponentCodeChanging(value);
                this.ReportPropertyChanging("ComponentCode");
                this._ComponentCode = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, false, 1);
                this.ReportPropertyChanged("ComponentCode");
                this.OnComponentCodeChanged();
            }
        }
        private string _ComponentCode;
        partial void OnComponentCodeChanging(string value);
        partial void OnComponentCodeChanged();
        /// <summary>
        /// There are no comments for Property MajorCode in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public decimal MajorCode
        {
            get
            {
                return this._MajorCode;
            }
            set
            {
                this.OnMajorCodeChanging(value);
                this.ReportPropertyChanging("MajorCode");
                this._MajorCode = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, 2, 0);
                this.ReportPropertyChanged("MajorCode");
                this.OnMajorCodeChanged();
            }
        }
        private decimal _MajorCode;
        partial void OnMajorCodeChanging(decimal value);
        partial void OnMajorCodeChanged();
        /// <summary>
        /// There are no comments for Property MinorCode in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute()]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public global::System.Nullable<short> MinorCode
        {
            get
            {
                return this._MinorCode;
            }
            set
            {
                this.OnMinorCodeChanging(value);
                this.ReportPropertyChanging("MinorCode");
                this._MinorCode = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("MinorCode");
                this.OnMinorCodeChanged();
            }
        }
        private global::System.Nullable<short> _MinorCode;
        partial void OnMinorCodeChanging(global::System.Nullable<short> value);
        partial void OnMinorCodeChanged();
        /// <summary>
        /// There are no comments for Property Concept in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public string Concept
        {
            get
            {
                return this._Concept;
            }
            set
            {
                this.OnConceptChanging(value);
                this.ReportPropertyChanging("Concept");
                this._Concept = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, false, 80);
                this.ReportPropertyChanged("Concept");
                this.OnConceptChanged();
            }
        }
        private string _Concept;
        partial void OnConceptChanging(string value);
        partial void OnConceptChanged();
        /// <summary>
        /// There are no comments for Property Definition in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute()]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public string Definition
        {
            get
            {
                return this._Definition;
            }
            set
            {
                this.OnDefinitionChanging(value);
                this.ReportPropertyChanging("Definition");
                this._Definition = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, true, 255);
                this.ReportPropertyChanged("Definition");
                this.OnDefinitionChanged();
            }
        }
        private string _Definition;
        partial void OnDefinitionChanging(string value);
        partial void OnDefinitionChanged();
        /// <summary>
        /// There are no comments for Care_component in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmRelationshipNavigationPropertyAttribute("CCC.Businesslayer", "FK_Nursing_Diagnosis_Care_component", "Care_component")]
        [global::System.Xml.Serialization.XmlIgnoreAttribute()]
        [global::System.Xml.Serialization.SoapIgnoreAttribute()]
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public Care_component Care_component
        {
            get
            {
                return ((global::System.Data.Objects.DataClasses.IEntityWithRelationships)(this)).RelationshipManager.GetRelatedReference<Care_component>("CCC.Businesslayer.FK_Nursing_Diagnosis_Care_component", "Care_component").Value;
            }
            set
            {
                ((global::System.Data.Objects.DataClasses.IEntityWithRelationships)(this)).RelationshipManager.GetRelatedReference<Care_component>("CCC.Businesslayer.FK_Nursing_Diagnosis_Care_component", "Care_component").Value = value;
            }
        }
        /// <summary>
        /// There are no comments for Care_component in the schema.
        /// </summary>
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public global::System.Data.Objects.DataClasses.EntityReference<Care_component> Care_componentReference
        {
            get
            {
                return ((global::System.Data.Objects.DataClasses.IEntityWithRelationships)(this)).RelationshipManager.GetRelatedReference<Care_component>("CCC.Businesslayer.FK_Nursing_Diagnosis_Care_component", "Care_component");
            }
            set
            {
                if ((value != null))
                {
                    ((global::System.Data.Objects.DataClasses.IEntityWithRelationships)(this)).RelationshipManager.InitializeRelatedReference<Care_component>("CCC.Businesslayer.FK_Nursing_Diagnosis_Care_component", "Care_component", value);
                }
            }
        }
    }
    /// <summary>
    /// There are no comments for CCC.Businesslayer.Nursing_Intervention in the schema.
    /// </summary>
    /// <KeyProperties>
    /// Language_Name
    /// ComponentCode
    /// MajorCode
    /// Concept
    /// </KeyProperties>
    [global::System.Data.Objects.DataClasses.EdmEntityTypeAttribute(NamespaceName="CCC.Businesslayer", Name="Nursing_Intervention")]
    [global::System.Runtime.Serialization.DataContractAttribute()]
    [global::System.Serializable()]
    public partial class Nursing_Intervention : global::System.Data.Objects.DataClasses.EntityObject
    {
        /// <summary>
        /// Create a new Nursing_Intervention object.
        /// </summary>
        /// <param name="language_Name">Initial value of Language_Name.</param>
        /// <param name="componentCode">Initial value of ComponentCode.</param>
        /// <param name="majorCode">Initial value of MajorCode.</param>
        /// <param name="concept">Initial value of Concept.</param>
        public static Nursing_Intervention CreateNursing_Intervention(string language_Name, string componentCode, decimal majorCode, string concept)
        {
            Nursing_Intervention nursing_Intervention = new Nursing_Intervention();
            nursing_Intervention.Language_Name = language_Name;
            nursing_Intervention.ComponentCode = componentCode;
            nursing_Intervention.MajorCode = majorCode;
            nursing_Intervention.Concept = concept;
            return nursing_Intervention;
        }
        /// <summary>
        /// There are no comments for Property Language_Name in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public string Language_Name
        {
            get
            {
                return this._Language_Name;
            }
            set
            {
                this.OnLanguage_NameChanging(value);
                this.ReportPropertyChanging("Language_Name");
                this._Language_Name = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, false, 25);
                this.ReportPropertyChanged("Language_Name");
                this.OnLanguage_NameChanged();
            }
        }
        private string _Language_Name;
        partial void OnLanguage_NameChanging(string value);
        partial void OnLanguage_NameChanged();
        /// <summary>
        /// There are no comments for Property ComponentCode in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public string ComponentCode
        {
            get
            {
                return this._ComponentCode;
            }
            set
            {
                this.OnComponentCodeChanging(value);
                this.ReportPropertyChanging("ComponentCode");
                this._ComponentCode = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, false, 1);
                this.ReportPropertyChanged("ComponentCode");
                this.OnComponentCodeChanged();
            }
        }
        private string _ComponentCode;
        partial void OnComponentCodeChanging(string value);
        partial void OnComponentCodeChanged();
        /// <summary>
        /// There are no comments for Property MajorCode in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public decimal MajorCode
        {
            get
            {
                return this._MajorCode;
            }
            set
            {
                this.OnMajorCodeChanging(value);
                this.ReportPropertyChanging("MajorCode");
                this._MajorCode = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, 2, 0);
                this.ReportPropertyChanged("MajorCode");
                this.OnMajorCodeChanged();
            }
        }
        private decimal _MajorCode;
        partial void OnMajorCodeChanging(decimal value);
        partial void OnMajorCodeChanged();
        /// <summary>
        /// There are no comments for Property MinorCode in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute()]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public global::System.Nullable<short> MinorCode
        {
            get
            {
                return this._MinorCode;
            }
            set
            {
                this.OnMinorCodeChanging(value);
                this.ReportPropertyChanging("MinorCode");
                this._MinorCode = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value);
                this.ReportPropertyChanged("MinorCode");
                this.OnMinorCodeChanged();
            }
        }
        private global::System.Nullable<short> _MinorCode;
        partial void OnMinorCodeChanging(global::System.Nullable<short> value);
        partial void OnMinorCodeChanged();
        /// <summary>
        /// There are no comments for Property Concept in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute(EntityKeyProperty=true, IsNullable=false)]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public string Concept
        {
            get
            {
                return this._Concept;
            }
            set
            {
                this.OnConceptChanging(value);
                this.ReportPropertyChanging("Concept");
                this._Concept = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, false, 80);
                this.ReportPropertyChanged("Concept");
                this.OnConceptChanged();
            }
        }
        private string _Concept;
        partial void OnConceptChanging(string value);
        partial void OnConceptChanged();
        /// <summary>
        /// There are no comments for Property Definition in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmScalarPropertyAttribute()]
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        public string Definition
        {
            get
            {
                return this._Definition;
            }
            set
            {
                this.OnDefinitionChanging(value);
                this.ReportPropertyChanging("Definition");
                this._Definition = global::System.Data.Objects.DataClasses.StructuralObject.SetValidValue(value, true, 255);
                this.ReportPropertyChanged("Definition");
                this.OnDefinitionChanged();
            }
        }
        private string _Definition;
        partial void OnDefinitionChanging(string value);
        partial void OnDefinitionChanged();
        /// <summary>
        /// There are no comments for Care_component in the schema.
        /// </summary>
        [global::System.Data.Objects.DataClasses.EdmRelationshipNavigationPropertyAttribute("CCC.Businesslayer", "FK_Nursing_Intervention_Care_component", "Care_component")]
        [global::System.Xml.Serialization.XmlIgnoreAttribute()]
        [global::System.Xml.Serialization.SoapIgnoreAttribute()]
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public Care_component Care_component
        {
            get
            {
                return ((global::System.Data.Objects.DataClasses.IEntityWithRelationships)(this)).RelationshipManager.GetRelatedReference<Care_component>("CCC.Businesslayer.FK_Nursing_Intervention_Care_component", "Care_component").Value;
            }
            set
            {
                ((global::System.Data.Objects.DataClasses.IEntityWithRelationships)(this)).RelationshipManager.GetRelatedReference<Care_component>("CCC.Businesslayer.FK_Nursing_Intervention_Care_component", "Care_component").Value = value;
            }
        }
        /// <summary>
        /// There are no comments for Care_component in the schema.
        /// </summary>
        [global::System.Runtime.Serialization.DataMemberAttribute()]
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public global::System.Data.Objects.DataClasses.EntityReference<Care_component> Care_componentReference
        {
            get
            {
                return ((global::System.Data.Objects.DataClasses.IEntityWithRelationships)(this)).RelationshipManager.GetRelatedReference<Care_component>("CCC.Businesslayer.FK_Nursing_Intervention_Care_component", "Care_component");
            }
            set
            {
                if ((value != null))
                {
                    ((global::System.Data.Objects.DataClasses.IEntityWithRelationships)(this)).RelationshipManager.InitializeRelatedReference<Care_component>("CCC.Businesslayer.FK_Nursing_Intervention_Care_component", "Care_component", value);
                }
            }
        }
    }
}
