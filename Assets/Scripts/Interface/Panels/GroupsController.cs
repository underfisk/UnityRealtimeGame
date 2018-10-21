using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GroupsController : MonoBehaviour
{
    protected enum HOption
    {
        Play = 0,
        Adventures = 1,
        Collections = 2
    }

    /// <summary>
    /// This layers are just an image glow that sets the choosed back to go to the next one or the previous one
    /// </summary>
    [SerializeField]
    public Button PlayBtn, AdventureBtn, CollectionBtn;

    /// <summary>
    /// Saves the header text ref
    /// </summary>
    [SerializeField]
    public Text HeaderPlay, HeaderAdventure, HeaderCollection;

    /// <summary>
    /// Group with Options inside
    /// </summary>
    private Dictionary<HOption, GameObject> Groups;

    /// <summary>
    /// We start at position 0
    /// </summary>
    private HOption currentGroupIndex = HOption.Play;

    protected void Start()
    {
        Groups = new Dictionary<HOption, GameObject>();

        //Find the groups
        Groups.Add(HOption.Play,transform.Find("PlayGroup").gameObject);
        Groups.Add(HOption.Adventures, transform.Find("AdventuresGroup").gameObject);
        Groups.Add(HOption.Collections, transform.Find("CollectionsGroup").gameObject);

        //Verify if every gameobject is defined
        if (PlayBtn == null) Debug.LogError("PlayBtn is missing");
        if (AdventureBtn == null) Debug.LogError("AdventureBtn is missing");
        if (CollectionBtn == null) Debug.LogError("CollectionBtn is missing");

        //Bind events
        if (PlayBtn != null)
            PlayBtn.onClick.AddListener(() => SetActiveGroup(HOption.Play));
        if (AdventureBtn != null)
            AdventureBtn.onClick.AddListener(() => SetActiveGroup(HOption.Adventures));
        if (CollectionBtn != null)
            CollectionBtn.onClick.AddListener(() => SetActiveGroup(HOption.Collections));

        //Set default
        SetActiveGroup(HOption.Play);
    }
    
    protected void SlideHeaderGlow(float x)
    {
        //We just change position x because we want it to slide under the options on the header
       // HeaderSlideGlow.transform.position = new Vector3(x, HeaderSlideGlow.transform.position.y, HeaderSlideGlow.transform.position.z);
        
    }

    private void SetActiveHeader(HOption _opt)
    {
        switch(_opt)
        {
            case HOption.Play:
                HeaderPlay.color = Color.white;
                HeaderAdventure.color = Color.grey;
                HeaderCollection.color = Color.grey;
                break;
            case HOption.Adventures:
                HeaderPlay.color = Color.grey;
                HeaderAdventure.color = Color.white;
                HeaderCollection.color = Color.grey;
                break;
            case HOption.Collections:
                HeaderPlay.color = Color.grey;
                HeaderAdventure.color = Color.grey;
                HeaderCollection.color = Color.white;
                break;
        }
    }

    protected void SetActiveGroup(HOption _opt)
    {
        Debug.Log("Setting active group with Option = " + _opt);
        switch(_opt)
        {
            case HOption.Play:
                GameObject playGroup = null;
                if (Groups.TryGetValue(_opt, out playGroup))
                {
                    CloseOpenGroups();
                    if (!playGroup.activeSelf)
                    {
                        SlideHeaderGlow(PlayBtn.transform.position.x);
                        SetActiveHeader(_opt);
                        playGroup.SetActive(true);
                    }
                }
                break;
            case HOption.Adventures:
                GameObject advGroup = null;
                if (Groups.TryGetValue(_opt, out advGroup))
                {
                    CloseOpenGroups();
                    if (!advGroup.activeSelf)
                    {
                        SlideHeaderGlow(AdventureBtn.transform.position.x);
                        SetActiveHeader(_opt);
                        advGroup.SetActive(true);
                    }
                }
                break;
            case HOption.Collections:
                break;
        }
    }
    
    protected void CloseOpenGroups()
    {
        foreach(var g in Groups)
        {
            if (g.Value.activeSelf)
                g.Value.SetActive(false);
        }
    }
}
