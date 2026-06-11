using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GFW.GameAbilitySystem.GameAttributeN
{
    public class AttributeModifierBase
    {
        public AttributeModifierBase() { }

        protected void CaptureAttribute<T>(string[] attributeNames, ECaptureAttributeSource captureAttributeSource) where T : GameAttributeSet
        {
            foreach (string name in attributeNames)
            {
                if (string.IsNullOrWhiteSpace(name)) continue;
                CaptureAttribute<T>(name, captureAttributeSource);
            }
        }

        protected void CaptureAttribute<T>(string attributeName, ECaptureAttributeSource captureAttributeSource) where T : GameAttributeSet
        {
            CaptureAttributeDeclare captureAttributeDeclare = new CaptureAttributeDeclare();
            captureAttributeDeclare.GameAttributeSetHashCode=typeof(T).GetHashCode();
            captureAttributeDeclare.GameAttributeName=attributeName;
            captureAttributeDeclare.CaptureAttributeSource=captureAttributeSource;
            CaptureAttributeDeclare.Add(captureAttributeDeclare);
        }

        public virtual void Excute(CapturedAttributes capturedAttributes,ModifiedResults outResult)
        {
            _ModifiedResults=outResult;
        }

        internal List<CaptureAttributeDeclare>CaptureAttributeDeclare=new List<CaptureAttributeDeclare>();

        internal CapturedAttributeField CapturedAttributes = new CapturedAttributeField();

        private ModifiedResults _ModifiedResults = new ModifiedResults();
        public ModifiedResults ModifiedResults => _ModifiedResults;
    }


    /// <summary>
    /// 捕获属性声明，用于描述想捕获的属性
    /// </summary>
    internal struct CaptureAttributeDeclare
    {
        public int GameAttributeSetHashCode;
        public string GameAttributeName;
        public ECaptureAttributeSource CaptureAttributeSource;
    }
    /// <summary>
    /// 捕获属性来源
    /// </summary>
    public enum ECaptureAttributeSource
    {
        /// <summary>
        /// 例：来自攻击者
        /// </summary>
        Source,
        /// <summary>
        /// 例：来自受击者
        /// </summary>
        Target,
    }

    /// <summary>
    /// 包装捕获到的属性的FieldInfo
    /// </summary>
    internal struct CapturedAttributeField : IEquatable<CapturedAttributeField>
    {
        public CapturedAttributeField(int ownerSetHashCode, FieldInfo fieldInfo)
        {
            _OwnerSetHashCode=ownerSetHashCode;
            _FieldInfo=fieldInfo;
        }
        /// <summary>
        /// 所属GameAttributeSet的类型哈希码
        /// </summary>
        int _OwnerSetHashCode;

        public int OwnerSetHashCode => _OwnerSetHashCode;

        FieldInfo _FieldInfo;
        public FieldInfo FieldInfo => _FieldInfo;


        public override int GetHashCode()
        {
            return HashCode.Combine(_OwnerSetHashCode, _FieldInfo.GetHashCode());
        }
        public bool Equals(CapturedAttributeField other)
        {
            //if (ReferenceEquals(other, null)) return false;

            if (_OwnerSetHashCode!=other._OwnerSetHashCode) return false;

            //if (ReferenceEquals(FieldInfo, null)&&ReferenceEquals(other.FieldInfo, null)) return true;

            if (ReferenceEquals(_FieldInfo, null)) return false;

            if (_FieldInfo.Name!=other._FieldInfo.Name) return false;

            return true;
        }
    }
    /// <summary>
    /// 已捕获到的属性集合
    /// </summary>
    public class CapturedAttributes
    {
        public CapturedAttributes()
        {
            _SourceAttribute = new Dictionary<int, List<CapturedAttributeField>>();
            _TargetAttribute = new Dictionary<int, List<CapturedAttributeField>>();
        }


        internal void AddAttributeToSource(int attributeSetHashCode, CapturedAttributeField capturedAttributeField)
        {
            List<CapturedAttributeField> fields;
            if (!_SourceAttribute.ContainsKey(attributeSetHashCode))
            {
                fields=new List<CapturedAttributeField>();
                _SourceAttribute[attributeSetHashCode]= fields;
            }
            else
            {
                fields=_SourceAttribute[attributeSetHashCode];
            }
            fields.Add(capturedAttributeField);
        }
        internal void AddAttributeToTarget(int attributeSetHashCode, CapturedAttributeField capturedAttributeField)
        {
            List<CapturedAttributeField> fields;
            if (!_TargetAttribute.ContainsKey(attributeSetHashCode))
            {
                fields=new List<CapturedAttributeField>();
                _TargetAttribute[attributeSetHashCode]= fields;
            }
            else
            {
                fields=_TargetAttribute[attributeSetHashCode];
            }
            fields.Add(capturedAttributeField);
        }

        Dictionary<int, List<CapturedAttributeField>> _SourceAttribute;
        Dictionary<int, List<CapturedAttributeField>> _TargetAttribute;
    }


    public enum EOperatorType
    {
        Additive,
        Multi,
        Divide,
        Override,
    }
    public enum EValueType
    {
        BaseValue,
        CurValue,
    }
    public class ModifiedResults
    {
        public ModifiedResults()
        {
            _Results=new List<Result>();
        }

        internal struct Result
        {
            public int AttributeSetHashCode;
            public float Value;
            public EValueType ValueType;
            public EOperatorType OperatorType;
            public string GameAttributeName;
        }

        public void Add<T>(string gameAttributeName, float outResult, EValueType valueType = EValueType.CurValue, EOperatorType operatorType = EOperatorType.Additive) where T : GameAttributeSet
        {
            Result result = new Result();
            result.AttributeSetHashCode = typeof(T).GetHashCode();
            result.Value = outResult;
            result.ValueType = valueType;
            result.OperatorType = operatorType;
            result.GameAttributeName=gameAttributeName;
            _Results.Add(result);
        }
        public void Add(int AttributeSetHashCode, string gameAttributeName, float outResult, EValueType valueType = EValueType.CurValue, EOperatorType operatorType = EOperatorType.Additive)
        {
            Result result = new Result();
            result.AttributeSetHashCode = AttributeSetHashCode;
            result.Value = outResult;
            result.ValueType = valueType;
            result.OperatorType = operatorType;
            result.GameAttributeName=gameAttributeName;
            _Results.Add(result);
        }
        internal List<Result> _Results;
    }
}
