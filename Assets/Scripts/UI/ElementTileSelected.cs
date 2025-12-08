using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sumfulla.Atomica
{
    [UxmlElement]
    partial class ElementTileSelected : VisualElement
    {
        private Label _enlargedLabel;
        private Label _tileAtomicNumber;
        //private string _tileName;


        [UxmlAttribute]
        public Length Size
        {
            get => _size;
            set
            {
                _size = value;
                _enlargedLabel.style.width = _size;
                _enlargedLabel.style.height = _size;
            }
        }
        private Length _size { get; set; } = new Length(100, LengthUnit.Percent);


        public ElementTileSelected()
        {
            // Set USS values
            StyleSheet uss = Resources.Load<StyleSheet>(UIRef.STYLESHEET_CORE);
            styleSheets.Add(uss);

            string et = "element-tile-selected";
            VisualTreeAsset etUxml = Resources.Load<VisualTreeAsset>(et);

            // Clone without the wrapper
            etUxml.CloneTree(contentContainer);
            
            _enlargedLabel = contentContainer.Q<Label>("element-tile__label");
            _tileAtomicNumber = _enlargedLabel.Q<Label>("element-atomic-number");
            pickingMode = PickingMode.Ignore;
        }

        public void Shrink()
        {
            _enlargedLabel.style.display = DisplayStyle.None;
        }

        public void Enlarge()
        {
            _enlargedLabel.style.display = DisplayStyle.Flex;
        }

        public void MakeBlank()
        {
            style.visibility = Visibility.Hidden;   
            _enlargedLabel.style.display = DisplayStyle.None;   
        }
        
        public void UpdateSize(float length)
        {
            style.width = length;
            style.height = length;
        }

        public void UpdateElementDetails(string symbol, string elName, int atomicNumber)
        {
            _enlargedLabel.text = symbol;
            _tileAtomicNumber.text = atomicNumber == 0 ? "" : atomicNumber.ToString();
        }

        public void UpdateElementDetails(string symbol, int atomicNumber)
        {
            _enlargedLabel.text = symbol;
            _tileAtomicNumber.text = atomicNumber.ToString();
            name = symbol;
        }

        public void UpdateColor(string tgc)
        {
            Color col = AppUtils.GetColor(tgc);
            _enlargedLabel.style.backgroundColor = Color.black;
            _enlargedLabel.style.borderTopColor = col;
            _enlargedLabel.style.borderBottomColor = col;
            _enlargedLabel.style.borderLeftColor = col;
            _enlargedLabel.style.borderRightColor = col;

            _enlargedLabel.style.color = col;
            _tileAtomicNumber.style.color = col;

        }

        public void UpdateColor(Color col)
        {
            _enlargedLabel.style.backgroundColor = Color.black;
            _enlargedLabel.style.borderTopColor = col;
            _enlargedLabel.style.borderBottomColor = col;
            _enlargedLabel.style.borderLeftColor = col;
            _enlargedLabel.style.borderRightColor = col;

            _enlargedLabel.style.color = col;
            _tileAtomicNumber.style.color = col;
        }

    }
}