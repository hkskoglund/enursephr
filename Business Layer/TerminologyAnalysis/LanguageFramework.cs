using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace eNursePHR.BusinessLayer
{
    public class langFramework
    {
        private string _languageName;
        private int _carePatternsShallow;
        private int _careComponentsShallow;
        private int _nursingDiagnosesShallow;
        private int _nursingInterventionsShallow;
        private int _outcomeTypesShallow;
        private int _actionTypesShallow;


        private bool _CarePatternsasExpected;
        private bool _CareComponentsasExpected;
        private bool _DiagnosesasExpected;
        private bool _InterventionsasExpected;
        private bool _OutcomeTypesasExpected;
        private bool _ActionTypesasExpected;

        private string _MetaInfo;

        public string MetaInfo
        {
            get { return this._MetaInfo; }
            set { this._MetaInfo = value; }
        }



        public bool CarePatternsAsExpected
        {
            get { return this._CarePatternsasExpected; }
            set { this._CarePatternsasExpected = value; }
        }

        public bool CareComponentsAsExpected
        {
            get { return this._CareComponentsasExpected; }
            set { this._CareComponentsasExpected = value; }
        }


        public bool DiagnosesAsExpected
        {
            get { return this._DiagnosesasExpected; }
            set { this._DiagnosesasExpected = value; }
        }

        public bool InterventionsAsExpected
        {
            get { return this._InterventionsasExpected; }
            set { this._InterventionsasExpected = value; }
        }

        public bool OutcomeTypesAsExpected
        {
            get { return this._OutcomeTypesasExpected; }
            set { this._OutcomeTypesasExpected = value; }
        }

        public bool ActionTypesAsExpected
        {
            get { return this._ActionTypesasExpected; }
            set { this._ActionTypesasExpected = value; }
        }

        public string LanguageName
        {
            get { return this._languageName.Trim(); }
            set { this._languageName = value.Trim(); }
        }

        public int CarePatternsShallow
        {
            get { return this._carePatternsShallow; }
            set { this._carePatternsShallow = value; }
        }

        public int CareComponentsShallow
        {
            get { return this._careComponentsShallow; }
            set { this._careComponentsShallow = value; }
        }

        public int NursingDiagnosesShallow
        {
            get { return this._nursingDiagnosesShallow; }
            set { this._nursingDiagnosesShallow = value; }
        }

        public int NursingInterventionsShallow
        {
            get { return this._nursingInterventionsShallow; }
            set { this._nursingInterventionsShallow = value; }
        }

        public int OutcomeTypesShallow
        {
            get { return this._outcomeTypesShallow; }
            set { this._outcomeTypesShallow = value; }
        }
        
        public int ActionTypesShallow
        {
            get { return this._actionTypesShallow; }
            set { this._actionTypesShallow = value; }
        }
    }
    
}
