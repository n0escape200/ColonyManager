using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject[] subMenus;
    int activMenu = 0;
    public CameraScript cameraComponent;

    public GameObject lumberBuilding;
    public GameObject ironBuilding;
    public GameObject stoneBuilding;
    public GameObject foodBuilding;
    public GameObject stockpileBuilding;
    public GameObject houseBuilding;
    public GameObject townhallBuilding;
    public GameObject wanderBuilding;
    public Transform stockpileParent;
    public Transform productionParent;
    public Transform housingParent;
    public Transform townhallParent;
    public Transform wanderParent;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (activMenu != 0)
            {
                activMenu = 0;
            }
            else
            {
                activMenu = 1;
            }
        }
        foreach (GameObject menu in subMenus)
        {
            if (menu.name == "BuildMenu")
            {
                GameObject buildMenu = menu;
                if (activMenu == 1)
                {
                    buildMenu.SetActive(true);
                }
                else
                {
                    buildMenu.SetActive(false);
                }
                if (buildMenu.activeSelf)
                {
                    foreach (GameObject descendant in GetAllDescendants(buildMenu.transform))
                    {
                        if (descendant.name == "buildingsBtn")
                        {
                            Button btn = descendant.GetComponent<Button>();
                            if (btn != null)
                            {
                                if (btn.interactable && Input.GetMouseButtonDown(0))
                                {
                                    // Optionally, check if the mouse is over the button
                                    // using RectTransformUtility or EventSystem.current.currentSelectedGameObject
                                    if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == btn.gameObject)
                                    {
                                        activMenu = 2;
                                        buildMenu.SetActive(false);
                                    }
                                }
                            }
                        }
                        if (descendant.name == "constructionBtn")
                        {
                            Button btn = descendant.GetComponent<Button>();
                            if (btn != null)
                            {
                                if (btn.interactable && Input.GetMouseButtonDown(0))
                                {
                                    // Optionally, check if the mouse is over the button
                                    // using RectTransformUtility or EventSystem.current.currentSelectedGameObject
                                    if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == btn.gameObject)
                                    {
                                        activMenu = 3;
                                        buildMenu.SetActive(false);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (menu.name == "BuildingsMenu")
            {
                GameObject buildingsMenu = menu;
                if (activMenu == 2)
                {
                    buildingsMenu.SetActive(true);
                }
                else
                {
                    buildingsMenu.SetActive(false);
                }
                if (buildingsMenu.activeSelf)
                {
                    foreach (GameObject descendant in GetAllDescendants(buildingsMenu.transform))
                    {
                        Button btn = descendant.GetComponent<Button>();
                        if (btn != null)
                        {
                            if (btn.interactable && Input.GetMouseButtonDown(0))
                            {
                                if (UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject == btn.gameObject)
                                {
                                    switch (descendant.name)
                                    {
                                        case "lumber":
                                            {
                                                GameObject obj = Instantiate(lumberBuilding, Vector3.zero, Quaternion.identity);
                                                obj.transform.SetParent(productionParent);
                                                cameraComponent.placeableObject = obj;
                                            }
                                            break;
                                        case "stone":
                                            {
                                                GameObject obj = Instantiate(stoneBuilding, Vector3.zero, Quaternion.identity);
                                                obj.transform.SetParent(productionParent);
                                                cameraComponent.placeableObject = obj;
                                            }
                                            break;
                                        case "iron":
                                            {
                                                GameObject obj = Instantiate(ironBuilding, Vector3.zero, Quaternion.identity);
                                                obj.transform.SetParent(productionParent);
                                                cameraComponent.placeableObject = obj;
                                            }
                                            break;
                                        case "food":
                                            {
                                                GameObject obj = Instantiate(foodBuilding, Vector3.zero, Quaternion.identity);
                                                obj.transform.SetParent(productionParent);
                                                cameraComponent.placeableObject = obj;
                                            }
                                            break;
                                        case "stockpile":
                                            {
                                                GameObject obj = Instantiate(stockpileBuilding, Vector3.zero, Quaternion.identity);
                                                obj.transform.SetParent(stockpileParent);
                                                cameraComponent.placeableObject = obj;
                                            }
                                            break;
                                        case "house":
                                            {
                                                GameObject obj = Instantiate(houseBuilding, Vector3.zero, Quaternion.identity);
                                                obj.transform.SetParent(housingParent);
                                                cameraComponent.placeableObject = obj;
                                            }
                                            break;
                                        case "townhall":
                                            {
                                                GameObject obj = Instantiate(townhallBuilding, Vector3.zero, Quaternion.identity);
                                                obj.transform.SetParent(townhallParent);
                                                cameraComponent.placeableObject = obj;
                                            }
                                            break;
                                        case "wander":
                                            {
                                                GameObject obj = Instantiate(wanderBuilding, Vector3.zero, Quaternion.identity);
                                                obj.transform.SetParent(wanderParent);
                                                cameraComponent.placeableObject = obj;
                                            }
                                            break;
                                        default:
                                            break;
                                    }

                                    activMenu = 0;
                                }
                            }
                        }
                    }
                }

            }

        }
    }
    private IEnumerable<GameObject> GetAllDescendants(Transform parent)
    {
        foreach (Transform child in parent)
        {
            yield return child.gameObject;
            foreach (GameObject descendant in GetAllDescendants(child))
            {
                yield return descendant;
            }
        }
    }

}
