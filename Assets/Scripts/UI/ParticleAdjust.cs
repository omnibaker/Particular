using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sumfulla.Atomica
{
    [UxmlElement]
    partial class ParticleAdjust : VisualElement
    {
        Color blank = new Color(0,0,0,0);
        public Label Count;

        public Label _label;
        private Button _decrease;
        private Button _increase;
        
        public Action OnIncrease;
        public Action OnDecrease;

        [UxmlAttribute]
        public Color LabelColor
        {
            get => _labelColor;
            set
            {
                _labelColor = value;
                if(_label != null)
                {
                    if (_labelColor != blank)
                    {
                        _label.style.backgroundColor = _labelColor;
                    }
                }
            }
        } 
        private Color _labelColor;

        [UxmlAttribute]
        public string ParticleLabel
        {
            get => _particleLabel;
            set
            {
                _particleLabel = value;
                if (_label != null && !string.IsNullOrWhiteSpace(_particleLabel))
                {
                    _label.text = _particleLabel;
                }
            }
        }
        private string _particleLabel;
        

        public ParticleAdjust()
        {
            // // Don't get in way of button
            // pickingMode = PickingMode.Ignore;

            // Set USS values
            StyleSheet uss = Resources.Load<StyleSheet>(UIRef.STYLESHEET_CORE);
            styleSheets.Add(uss);

            string pa = "particle-adj";
            VisualTreeAsset paUxml = Resources.Load<VisualTreeAsset>(pa);

            // Clone WITHOUT the stupid wrapper
            paUxml.CloneTree(contentContainer);

            // Now query directly from *this* (or contentContainer)
            Count     = this.Q<Label>(pa + "__count");
            _label    = this.Q<Label>(pa + "__label");
            _increase = this.Q<Button>(pa + "__increase");
            _decrease = this.Q<Button>(pa + "__decrease");

            // Add actions
            _increase.clicked += () => OnIncrease?.Invoke();
            _decrease.clicked += () => OnDecrease?.Invoke();

            //style.maxWidth = 200;
            //style.flexGrow = 1;
        }
    }
}

