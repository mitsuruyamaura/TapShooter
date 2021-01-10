using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ElementCompatibilityHelper
{
    public static bool GetElementCompatibility(ElementType attackElementType, ElementType defenseElementType) {
        if(attackElementType == ElementType.Black) {
            if (defenseElementType == ElementType.White) {
                return true;
            } else {
                return false;
            }
        } else if (attackElementType == ElementType.Blue) {
            if (defenseElementType == ElementType.Red) {
                return true;
            } else {
                return false;
            }
        } else if (attackElementType == ElementType.Green) {
            if (defenseElementType == ElementType.Blue) {
                return true;
            } else {
                return false;
            }
        } else if (attackElementType == ElementType.Red) {
            if (defenseElementType == ElementType.Green) {
                return true;
            } else {
                return false;
            }
        } else if (attackElementType == ElementType.White) {
            if (defenseElementType == ElementType.Black) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    } 
}
