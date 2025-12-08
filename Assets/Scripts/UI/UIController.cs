using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sumfulla.Atomica
{
    public class UIController : MonoBehaviour
    {
        //[SerializeField] private Camera _atomCam;
        private VisualElement _root;
        private Button _toeDisplay;
        private Button _toeHide;
        private Button _infoPanelClose;
        private Button _infoPopupButton;
        private VisualElement _toeRoot;
        private VisualElement _infoPanel;
        private Label _infoName;
        private Label _infoType; 
        private Label _selectingLabel;
        private ElementTileSelected _selectingTile;
        private Label _infoElectroConfig; 
        private Label _infoAtomicDescrption; 
        private Label _infoAtomicNumber; 
        private Label _infoAtomicMass; 
        private Label _messenger;
        private Label _symbolLabel;
        private Label _symbolCharge;
        internal ParticleAdjust AdjustProton;
        internal ParticleAdjust AdjustNeutron;
        internal ParticleAdjust AdjustElectron;

        private Coroutine _messengerFading;

        public static Action<int> SelectElement;

        private void OnEnable()
        {
            BohrRutherfordModel.UpdateElement += UpdateInfo;
            BohrRutherfordModel.UpdateIsotopeName += UpdateName;
            BohrRutherfordModel.UpdateParticleValues += UpdateParticleValues;
            BohrRutherfordModel.UpdateElecConfig += UpdateElectroconfig;
            BohrRutherfordModel.UpdateSymbol += UpdateSymbol;
            BohrRutherfordModel.UpdateMessenger += UpdateMessenger;
        }
        private void OnDisable()
        {
            BohrRutherfordModel.UpdateElement -= UpdateInfo;
            BohrRutherfordModel.UpdateIsotopeName -= UpdateName;
            BohrRutherfordModel.UpdateParticleValues -= UpdateParticleValues;
            BohrRutherfordModel.UpdateElecConfig -= UpdateElectroconfig;
            BohrRutherfordModel.UpdateSymbol -= UpdateSymbol;
            BohrRutherfordModel.UpdateMessenger -= UpdateMessenger;
        }

        private void Awake()
        {
            // Get root document element
            _root = GetComponent<UIDocument>().rootVisualElement;

            // Tweak adjusters
            AdjustProton = _root.Q<ParticleAdjust>("particle-adjust__proton");
            AdjustNeutron = _root.Q<ParticleAdjust>("particle-adjust__neutron");
            AdjustElectron = _root.Q<ParticleAdjust>("particle-adjust__electron");

            // Table of elements
            _toeRoot = _root.Q("toe");

            // Buttons
            _toeDisplay = _root.Q<Button>("toe-display");
            _toeHide = _toeRoot.Q<Button>("toe-hide");
            _infoPopupButton = _root.Q<Button>("info-popup-button");
            _infoPanelClose = _root.Q<Button>("info-panel-close");
            _infoPanelClose = _root.Q<Button>("info-panel-close");

            // Displays
            _symbolLabel = _root.Q<Label>("symbol-label");
            _symbolCharge = _root.Q<Label>("symbol-charge");
            _infoName = _root.Q<Label>("info-name");
            _infoType = _root.Q<Label>("info-type");
            _infoAtomicDescrption = _root.Q<Label>("info-atomic-description");
            _messenger = _root.Q<Label>("messenger");
            _infoAtomicNumber = _root.Q<Label>("info-atomic-number");
            _infoAtomicMass = _root.Q<Label>("info-atomic-mass");
            _infoElectroConfig = _root.Q<Label>("info-electroconfig");
            _infoPanel = _root.Q("info-panel");
        }
        
        /// <summary>
        /// Sets up periodic table and actions
        /// </summary>
        public IEnumerator Init()
        {
            // What a frame so layout size can be calculated
            yield return new WaitForEndOfFrame();

            // Crreate Periodic Table object
            CreateTOE();
            _infoPopupButton.clicked += ToggleInfoPanel;
            _infoPanelClose.clicked += ToggleInfoPanel;
        }


        /// <summary>
        /// Updates/disables the selection displayed when hovering over element in periodic table
        /// </summary>
        private void UpdateSelectingDisplay(string selectingName, Color clr, string symbol, int atomicNumber)
        {
            if (clr == Color.clear)
            {
                HideSelectingInfo();
            }
            else
            {
                _selectingTile.style.display = DisplayStyle.Flex;
                _selectingLabel.text = selectingName;
                _selectingTile.UpdateElementDetails(symbol, selectingName, atomicNumber);
                _selectingTile.UpdateColor(clr);
            }
        }

        /// <summary>
        /// Creates periodic table display with element blocks as buttons to update current element
        /// </summary>
        public void CreateTOE()
        {
            // Bind toggles
            _toeDisplay.clicked += ToggleTOE;
            _toeHide.clicked += ToggleTOE;

            // Identify elements from UI
            _selectingLabel = _toeRoot.Q<Label>("selecting-label");
            VisualElement toeContainer = _toeRoot.Q("toe-container");
            VisualElement selecting = _toeRoot.Q<Label>("selecting");
            VisualElement toeContainerSelected = _toeRoot.Q("toe-container__selected");
            toeContainerSelected.pickingMode = PickingMode.Ignore;

            // Find selecting tile and update style
            _selectingTile = _toeRoot.Q<ElementTileSelected>("selecting-tile");
            SetupSelectingTile(_selectingTile);
            HideSelectingInfo();
                
            // Calculate lenght of each element block
            int width = Mathf.CeilToInt(_root.layout.width);
            float tileLength = width / 20f;

            // Loop through each row/column and update each block to create TOE display
            int atomicNo = 1;
            for (int row = 1; row <= 9; row++)
            {
                toeContainerSelected[row - 1].pickingMode = PickingMode.Ignore;

                for (int col = 1; col <= 18; col++)
                {
                    // First block is placer
                    if (col == 1)
                    {
                        SetRowHeight(toeContainer[row - 1], tileLength);
                        SetRowHeight(toeContainerSelected[row - 1], tileLength);
                    }

                    // Ignores any other gaps in the table and creates empty block as a placer
                    if (IsEmptyBlock(row, col))
                    {
                        toeContainer[row - 1].Add(MakeBlankTile(tileLength));
                        toeContainerSelected[row - 1].Add(MakeBlankTile(tileLength));
                        continue;
                    }

                    // Button that is displayed
                    ElementTileButton et = new ElementTileButton();
                    et.UpdateSize(tileLength);

                    // The larger expand version when pressed/hovered over
                    ElementTileSelected ets = new ElementTileSelected();
                    ets.UpdateSize(tileLength);

                    // Offset the table position to center properly
                    float offset = 64f - ((tileLength - 2f) / 2f);
                    ets[0].style.translate = new Vector3(-offset, -offset, 0);

                    // Update appearance and behaviour displayed tile
                    JSONData.JSONElement e = JSONData.JsonElements[atomicNo - 1];
                    et.UpdateElementDetails(e.Symbol, e.Name, e.AtomicNo);
                    et.UpdateColor(e.TypeName);
                    et.DisplayEnlarged += ets.Enlarge;
                    et.UpdateSelecting += UpdateSelectingDisplay;
                    et.RemoveEnlarged += ets.Shrink;
                    et.UpdateButton(() =>
                    {
                        SelectElement.Invoke(e.AtomicNo);
                        DisplayTOE(false);
                    });
                    toeContainer[row - 1].Add(et);

                    // Update appearance and behaviour enlarged tile
                    ets.UpdateElementDetails(e.Symbol, e.AtomicNo);
                    ets.UpdateColor(e.TypeName);
                    ets.Shrink();
                    toeContainerSelected[row - 1].Add(ets);

                    // Manage how elements are sequenced in the displayed
                    if(atomicNo == 56)  atomicNo = 71;   // Lanthanides skip
                    else if (atomicNo == 88) atomicNo = 103;  // Actinides skip
                    else if (atomicNo == 118) atomicNo = 57;   // Go to Lanthanides
                    else if (atomicNo == 70) atomicNo = 89;   // Go to Actinides
                    else if (atomicNo == 102)
                    {
                        // Add three remaining blank blocks
                        toeContainer[row - 1].Add(MakeBlankTile(tileLength));
                        toeContainer[row - 1].Add(MakeBlankTile(tileLength));
                        toeContainerSelected[row - 1].Add(MakeBlankTile(tileLength));
                        toeContainerSelected[row - 1].Add(MakeBlankTile(tileLength));

                        // Hide the selection UI
                        DisplayTOE(false);

                        // Kill loop and leave
                        return;
                    }
                    else
                    {
                        atomicNo++;
                    }

                }
            }
        }

        /// <summary>
        /// Simple method of row resizing to avoid repetition
        /// </summary>
        private void SetRowHeight(VisualElement row, float tileLength)
        {
            row.style.height = tileLength;
            row.style.flexShrink = 0;
        }

        /// <summary>
        /// Configures display of element displayed when hovering over element in periodic table
        /// </summary>
        private void SetupSelectingTile(ElementTileSelected tile)
        {
            Label selectingTileLabel = _selectingTile.Q<Label>("element-tile__label");
            selectingTileLabel.style.borderTopWidth = 10;
            selectingTileLabel.style.borderBottomWidth = 10;
            selectingTileLabel.style.borderRightWidth = 10;
            selectingTileLabel.style.borderLeftWidth = 10;
            selectingTileLabel.style.fontSize = 200;

            Label selectingTileAtomicNo = _selectingTile.Q<Label>("element-atomic-number");
            selectingTileAtomicNo.style.fontSize = 40;
        }

        /// <summary>
        /// Creates and returns a blank spacer tile when element block doesn't require element data
        /// </summary>
        private ElementTileButton MakeBlankTile(float tileLength)
        {
            ElementTileButton blankTile = new ElementTileButton();
            blankTile.UpdateSize(tileLength);
            blankTile.MakeBlank();
            return blankTile;
        }

        /// <summary>
        /// Toggles display of periodic table UI based on current state
        /// </summary>
        private void ToggleTOE()
        {
            if (_toeRoot.style.display == DisplayStyle.Flex)
            {
                DisplayTOE(false);
            }
            else
            {
                HideSelectingInfo();
                DisplayTOE(true);
            }
        }


        /// <summary>
        /// Enables/disables periodic table UI based on parameter
        /// </summary>
        private void DisplayTOE(bool display)
        {
            _toeRoot.style.display = display ? DisplayStyle.Flex : DisplayStyle.None;
        }


        /// <summary>
        /// Simple method that hides display of selection preview to avoid repetition
        /// </summary>
        private void HideSelectingInfo()
        {
            _selectingTile.style.display = DisplayStyle.None;
            _selectingLabel.text = "";
        }


        /// <summary>
        /// Toggles display of info panel based on current state
        /// </summary>
        private void ToggleInfoPanel()
        {
            if (_infoPanel.style.display == DisplayStyle.Flex)
            {
                _infoPanel.style.display = DisplayStyle.None;
            }
            else
            {
                _infoPanel.style.display = DisplayStyle.Flex;
            }
        }

        /// <summary>
        /// Returns true if fits current row/column criteria to skip block as empty
        /// </summary>
        private bool IsEmptyBlock(int row, int col)
        {
            // Blocks to be blank with no content
            if ((row == 1 && col > 1 && col < 18) ||
            ((row == 2 || row == 3) && col > 2 && col < 13) ||
            ((row == 8 || row == 9) && (col <= 2 || col >= 17))) return true;

            return false;
        }

        /// <summary>
        /// Updates particle count display in UI
        /// </summary>
        public void UpdateParticleValues(int protons, int neutrons, int electrons)
        {
            AdjustProton.Count.text = protons.ToString();
            AdjustNeutron.Count.text = neutrons.ToString();
            AdjustElectron.Count.text = electrons.ToString();
        }

        /// <summary>
        /// Updates panel info on element's propeties/info
        /// </summary>
        public void UpdateInfo(JSONData.JSONElement element)
        {
            UpdateName(element.Name);
            _infoType.text = element.TypeGroupRef;
            _infoAtomicDescrption.text = element.Description;
            _infoAtomicNumber.text = $"Atomic Number: {element.AtomicNo}";
            _infoAtomicMass.text = $"Atomic Mass: {element.AtomicMass}";
        }

        /// <summary>
        /// Updates element name (sometimes isotopes can have different names)
        /// </summary>
        public void UpdateName(string newName)
        {
            _infoName.text = newName;
        }

        /// <summary>
        /// Update symbol/charge labels in main UI
        /// </summary>
        public void UpdateSymbol(string symbol, string charge)
        {
            _symbolLabel.text = symbol;
            _symbolCharge.text = charge;
        }

        /// <summary>
        /// Update electro-configuration labels in main UI
        /// </summary>
        public void UpdateElectroconfig(string ec)
        {
            _infoElectroConfig.text = ec;
        }

        /// <summary>
        /// Stops any current message fade and starts new temporary message output
        /// </summary>
        public void UpdateMessenger(string msg)
        {
            // Kill any current message coroutine
            if(_messengerFading != null)
            {
                StopCoroutine(_messengerFading);
            }

            _messengerFading = StartCoroutine(StartMessenger(msg));
        }

        /// <summary>
        /// Updates message field and then fades out
        /// </summary>
        public IEnumerator StartMessenger(string msg)
        {
            // Update text
            _messenger.text = msg;

            // Make visible
            _messenger.style.opacity = 1.0f;

            // Brief hold of message
            yield return new WaitForSeconds(2.0f);

            // Fade out into further opacity, then remove message
            float t = 1.0f;
            while (t > 0.0f)
            {
                t -= Time.deltaTime * 0.5f;
                _messenger.style.opacity = t;
                yield return null;
            }

            _messenger.style.opacity = 0;
            _messenger.text = "";
        }
    }
}