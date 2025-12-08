using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sumfulla.Atomica
{
    [UxmlElement]
    partial class ElementTileButton : VisualElement
    {
        public Action DisplayEnlarged;
        public Action RemoveEnlarged;
        public Action<string, Color, string, int> UpdateSelecting;
        public Action TileSelected;
        private Button _tileBase;
        private Label _tileAtomicNumber;
        private string _symbol;
        private string _elementName;
        private int _atomicNumber;
        private Color _color;

        public ElementTileButton()
        {
            // Set USS values
            StyleSheet uss = Resources.Load<StyleSheet>(UIRef.STYLESHEET_CORE);
            styleSheets.Add(uss);

            string et = "element-tile-button";
            VisualTreeAsset etUxml = Resources.Load<VisualTreeAsset>(et);

            // Clone without the wrapper
            etUxml.CloneTree(contentContainer);
            
            _tileBase = contentContainer.Q<Button>("element-tile-button");
            _tileAtomicNumber = _tileBase.Q<Label>("element-atomic-number");

            _tileBase.RegisterCallback<PointerEnterEvent>((evt) =>
            {
                DisplayEnlarged?.Invoke();
                UpdateSelecting?.Invoke(_elementName, _color, _symbol, _atomicNumber);
            });

            _tileBase.RegisterCallback<PointerLeaveEvent>((evt) =>
            {
                RemoveEnlarged?.Invoke();
                UpdateSelecting?.Invoke("", Color.clear, "", 0);
            });
        }

        public void AddRelease(MouseEnterEvent evt, Action action)
        {
            RegisterCallback<MouseEnterEvent>((evt) =>
            {
                action.Invoke();
            });
        }

        public void MakeBlank()
        {
            style.visibility = Visibility.Hidden;   
            _tileBase.style.display = DisplayStyle.None;   
        }

        public void MakeReferenceBlock(bool isActinides)
        {
            UpdateColor(isActinides ? "Actinides" : "Lanthanides");
        }

        public void UpdateSize(float length)
        {
            style.width = length;
            style.height = length;
        }

        public void UpdateElementDetails(string symbol, string elName, int atomicNumber)
        {
            _tileBase.text = symbol;
            _atomicNumber = atomicNumber;
            _tileAtomicNumber.text = _atomicNumber.ToString();
            _symbol = symbol;
            _elementName = elName;
            name = symbol;
        }

        public void UpdateButton(Action action)
        {
            _tileBase.clicked += action;
            TileSelected += action;
            TileSelected += () => { Debug.Log("Tile selected via mouse up"); };
            _tileBase.name = _symbol;

            _tileBase.RegisterCallback<PointerUpEvent>((evt) =>
            {
                if (panel == null) return;

                // Hit-test at current pointer position
                Button hovered = panel.Pick(evt.position) as Button;
                if (hovered != null)
                {
                    // Invoke button clicked event
                    using (NavigationSubmitEvent submitEvt = NavigationSubmitEvent.GetPooled())
                    {
                        submitEvt.target = hovered;
                        hovered.SendEvent(submitEvt);
                    }
                }

            });
        }

        public void UpdateColor(string tgc)
        {
            _color = AppUtils.GetColor(tgc);
            _tileBase.style.backgroundColor = Color.black;
            _tileBase.style.borderTopColor = _color;
            _tileBase.style.borderBottomColor = _color;
            _tileBase.style.borderLeftColor = _color;
            _tileBase.style.borderRightColor = _color;

            //Color col = Color.Lerp(_color, Color.white, AppColor.LABEL_WHITENING);
            _tileBase.style.color = _color;
            _tileAtomicNumber.style.color = _color;
        }
    }

    public enum TileColorGroup
    {
        Halogens,
        NobelGases,
        AlkaliMetals,
        AlkalineEarthMetals,
        Metalloids,
        OtherNonMetals
    }
}