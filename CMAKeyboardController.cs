//------------------------------------------------------------------------------
// GTA V Keyboard Controller by Razorwings18
//
// What you should use for this to work:
//
// - At the *BEGINNING* of onKeyDown:
// CMAKeyboardController.RegisterKeyDown(e.KeyCode);
//
// - At the *BEGINNING* of onKeyDown (Optional, only if you want to see what key was actually pressed - e.g., Keys.LShift instead of Keys.ShiftKey):
// Keys _trueKeyDown = CMAKeyboardController.GetTrueKeyDown(e.KeyCode);
//
// - At the *BEGINNING* of onKeyUp
// Keys _trueKeyUp = CMAKeyboardController.GetTrueKeyUp(e.KeyCode);
// 
// - At the *END* of onKeyUp
// CMAKeyboardController.UnregisterKeyDown(_trueKeyUp);
//
// --------------------------------------------------------------------------------
//
// - To check whether a key or key combination is pressed DOWN (in onKeyDown)
// CMAKeyboardController.VerifyKeyDownWithModifier(<main key>, <modifier key>)
// Pass KeyCode.None to <modifier key> if not a combination
//
// - To check whether a key combination is pressed UP (in onKeyUp)
// CMAKeyboardController.VerifyKeyUpWithModifier(<_trueKeyUp>, <main key>, <modifier key>);
// Pass KeyCode.None to <modifier key> if not a combination
//------------------------------------------------------------------------------

using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace KeyboardController
{
    public  class CMAKeyboardController
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(Keys key);
        
        public List<Keys> keysCurrentlyDown = new List<Keys>();

        public  void RegisterKeyDown(Keys key){
            RecheckModifierStatus();

            Keys _trueKey = GetTrueKeyDown(key);
            
            if (!keysCurrentlyDown.Contains(_trueKey)){
                keysCurrentlyDown.Add(_trueKey);
            }
        }
        
        public  void UnregisterKeyDown(Keys key){
            Keys _trueKey = GetTrueKeyUp(key);
            
            keysCurrentlyDown.Remove(_trueKey);
        }
        
        public  Keys GetTrueKeyDown(Keys key){
            Keys _trueKey = Keys.None;
            
            if (key == Keys.ShiftKey) {
                if (GetAsyncKeyState(Keys.LShiftKey) < 0) _trueKey = Keys.LShiftKey;
                if (GetAsyncKeyState(Keys.RShiftKey) < 0) _trueKey = Keys.RShiftKey;
            }
            else{
                if (key == Keys.ControlKey) {
                    if (GetAsyncKeyState(Keys.LControlKey) < 0) _trueKey = Keys.LControlKey;
                    if (GetAsyncKeyState(Keys.RControlKey) < 0) _trueKey = Keys.RControlKey;
                }
                else{
                    if (key == Keys.Menu) {
                        if (GetAsyncKeyState(Keys.LMenu) < 0) _trueKey = Keys.LMenu;
                        if (GetAsyncKeyState(Keys.RMenu) < 0) _trueKey = Keys.RMenu;
                    }
                    else{
                        _trueKey = key;
                    }
                }
            }
            
            return _trueKey;
        }
        
        public  Keys GetTrueKeyUp(Keys key){
            Keys _trueKey = Keys.None;
            
            if (key == Keys.ShiftKey) {
                if ((keysCurrentlyDown.Contains(Keys.LShiftKey)) && (GetAsyncKeyState(Keys.LShiftKey) >= 0)) _trueKey = Keys.LShiftKey;
                if ((keysCurrentlyDown.Contains(Keys.RShiftKey)) && (GetAsyncKeyState(Keys.RShiftKey) >= 0)) _trueKey = Keys.RShiftKey;
            }
            else{
                if (key == Keys.ControlKey) {
                    if ((keysCurrentlyDown.Contains(Keys.RControlKey)) && (GetAsyncKeyState(Keys.LShiftKey) >= 0)) _trueKey = Keys.RControlKey;
                    if ((keysCurrentlyDown.Contains(Keys.LControlKey)) && (GetAsyncKeyState(Keys.RShiftKey) >= 0)) _trueKey = Keys.LControlKey;
                }
                else{
                    if (key == Keys.Menu) {
                        if ((keysCurrentlyDown.Contains(Keys.RMenu)) && (GetAsyncKeyState(Keys.LShiftKey) >= 0)) _trueKey = Keys.RMenu;
                        if ((keysCurrentlyDown.Contains(Keys.LMenu)) && (GetAsyncKeyState(Keys.RShiftKey) >= 0)) _trueKey = Keys.LMenu;
                    }
                    else{
                        _trueKey = key;
                    }
                }
            }
            
            return _trueKey;
        }

        public  bool VerifyKeyDownWithModifier(Keys mainKey, Keys modifierKey){
            if (modifierKey == Keys.None){
                // Make sure the option is NOT activated if any common modifier is pressed
                if (keysCurrentlyDown.Contains(mainKey) 
                    && (!keysCurrentlyDown.Contains(Keys.LControlKey)) && (!keysCurrentlyDown.Contains(Keys.RControlKey))
                    && (!keysCurrentlyDown.Contains(Keys.LMenu)) && (!keysCurrentlyDown.Contains(Keys.RMenu))
                    && (!keysCurrentlyDown.Contains(Keys.LShiftKey)) && (!keysCurrentlyDown.Contains(Keys.RShiftKey))){
                    return true;
                }
            }
            else{
                if (keysCurrentlyDown.Contains(mainKey) && keysCurrentlyDown.Contains(modifierKey)){
                    return true;
                }
            }
            
            return false;
        }
        
        public  bool VerifyKeyUpWithModifier(Keys releasedKey, Keys mainKey, Keys modifierKey){
            if ((releasedKey == mainKey) || (releasedKey == modifierKey)){
                if (modifierKey == Keys.None){
                    // Make sure the option is NOT activated if any common modifier is pressed
                    if (keysCurrentlyDown.Contains(mainKey) 
                        && (!keysCurrentlyDown.Contains(Keys.LControlKey)) && (!keysCurrentlyDown.Contains(Keys.RControlKey))
                        && (!keysCurrentlyDown.Contains(Keys.LMenu)) && (!keysCurrentlyDown.Contains(Keys.RMenu))
                        && (!keysCurrentlyDown.Contains(Keys.LShiftKey)) && (!keysCurrentlyDown.Contains(Keys.RShiftKey))){
                        return true;
                    }
                }
                else{
                    if (keysCurrentlyDown.Contains(mainKey) && keysCurrentlyDown.Contains(modifierKey)){
                        return true;
                    }
                }
            }
            
            return false;
        }

        private void RecheckModifierStatus(){
            // This is necessary because GTA does not detect a key up sometimes, i.e., when alt-tabbing out of the game

            int i=0;
            while (i < keysCurrentlyDown.Count){
                if ((keysCurrentlyDown[i] == Keys.LControlKey) || (keysCurrentlyDown[i] == Keys.RControlKey)
                    || (keysCurrentlyDown[i] == Keys.LMenu) || (keysCurrentlyDown[i] == Keys.RMenu)
                    || (keysCurrentlyDown[i] == Keys.LShiftKey) || (keysCurrentlyDown[i] == Keys.RShiftKey)){

                    if (GetAsyncKeyState(keysCurrentlyDown[i]) >= 0){
                        // The key is actually up; remove it from the keysCurrentlyDown list
                        keysCurrentlyDown.RemoveAt(i);
                    }
                    else{
                        ++i;
                    }
                }
                else{
                    ++i;
                }
            }
        }
    }
}

