/*
 * This script was adapted from Meta XR Interaction SDK Essentials
 * 
 * Additional properties were added to include the z offset of a 3d button.
 */

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.Assertions;

namespace Oculus.Interaction
{
    [ExecuteAlways]
    public class RoundedBoxProperties : MonoBehaviour
    {
        [SerializeField]
        private MaterialPropertyBlockEditor _editor;

        [SerializeField]
        private float _width = 1.0f;

        [SerializeField]
        private float _height = 1.0f;

        [SerializeField]
        private float _depth = 1.0f;

        [SerializeField]
        private Color _color = Color.white;

        [SerializeField]
        private Color _borderColor = Color.black;

        [SerializeField]
        private float _radiusTopLeft;

        [SerializeField]
        private float _radiusTopRight;

        [SerializeField]
        private float _radiusBottomLeft;

        [SerializeField]
        private float _radiusBottomRight;

        [SerializeField]
        private float _borderInnerRadius;

        [SerializeField]
        private float _borderOuterRadius;

        #region Properties

        public float Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                UpdateSize();
            }
        }

        public float Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                UpdateSize();
            }
        }

        public float Depth
        {
            get
            {
                return _depth;
            }
            set
            {
                _depth = value;
                UpdateSize();
            }
        }

        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }

        public Color BorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                _borderColor = value;
            }
        }

        public float RadiusTopLeft
        {
            get
            {
                return _radiusTopLeft;
            }
            set
            {
                _radiusTopLeft = value;
            }
        }

        public float RadiusTopRight
        {
            get
            {
                return _radiusTopRight;
            }
            set
            {
                _radiusTopRight = value;
            }
        }

        public float RadiusBottomLeft
        {
            get
            {
                return _radiusBottomLeft;
            }
            set
            {
                _radiusBottomLeft = value;
            }
        }

        public float RadiusBottomRight
        {
            get
            {
                return _radiusBottomRight;
            }
            set
            {
                _radiusBottomRight = value;
            }
        }

        public float BorderInnerRadius
        {
            get
            {
                return _borderInnerRadius;
            }
            set
            {
                _borderInnerRadius = value;
            }
        }

        public float BorderOuterRadius
        {
            get
            {
                return _borderOuterRadius;
            }
            set
            {
                _borderOuterRadius = value;
                UpdateSize();
            }
        }

        #endregion

        private readonly int _colorShaderID = Shader.PropertyToID("_Color");
        private readonly int _borderColorShaderID = Shader.PropertyToID("_BorderColor");
        private readonly int _radiiShaderID = Shader.PropertyToID("_Radii");
        private readonly int _dimensionsShaderID = Shader.PropertyToID("_Dimensions");

        protected virtual void Awake()
        {
            UpdateSize();
            UpdateMaterialPropertyBlock();
        }

        protected virtual void Start()
        {
            this.AssertField(_editor, nameof(_editor));
            UpdateSize();
            UpdateMaterialPropertyBlock();
        }

        private void UpdateSize()
        {
            transform.localScale = new Vector3(_width + _borderOuterRadius * 2,
                                               _height + _borderOuterRadius * 2,
                                               _depth + _borderOuterRadius * 2);
            UpdateMaterialPropertyBlock();
        }

        private void UpdateMaterialPropertyBlock()
        {
            if (_editor == null)
            {
                _editor = GetComponent<MaterialPropertyBlockEditor>();
                if (_editor == null)
                {
                    return;
                }
            }

            MaterialPropertyBlock block = _editor.MaterialPropertyBlock;

            block.SetColor(_colorShaderID, _color);
            block.SetColor(_borderColorShaderID, _borderColor);
            block.SetVector(_radiiShaderID,
                                             new Vector4(
                                                _radiusTopRight,
                                                _radiusBottomRight,
                                                _radiusTopLeft,
                                                _radiusBottomLeft
                                             ));
            block.SetVector(_dimensionsShaderID,
                                             new Vector4(
                                                transform.localScale.x,
                                                transform.localScale.y,
                                                _borderInnerRadius,
                                                _borderOuterRadius
                                             ));

            _editor.UpdateMaterialPropertyBlock();
        }

        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            // If the current object is a Prefab source asset (not even an instance of it),
            // we do not want to apply these updates on validation,
            // as it may result on the prefab being marked as dirty
            // which would throw an error if the prefab lives in a package
            if (PrefabUtility.IsPartOfPrefabAsset(gameObject))
            {
                return;
            }
#endif

            UpdateSize();
            UpdateMaterialPropertyBlock();
        }
    }
}
