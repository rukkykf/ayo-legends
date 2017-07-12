using UnityEngine;
using System.Collections;

public class introScene : MonoBehaviour {

    private Animator    _animator;
    private CanvasGroup _canvasGroup;

    public introScene nextMenu; 

    public bool isOpen {
        get { return _animator.GetBool("isOpen"); }
        set { _animator.SetBool("isOpen", value); }
    }

	// Use this for initialization
	void Awake () {
        _animator = GetComponent<Animator>();
        _canvasGroup = GetComponent<CanvasGroup>();

       
	}

	
	// Update is called once per frame
	void Update () {
	    if(_animator.GetCurrentAnimatorStateInfo(0).IsName("introClip") || _animator.GetCurrentAnimatorStateInfo(0).IsName("creditIntroClip")) {
            _canvasGroup.blocksRaycasts = _canvasGroup.interactable = true;
            _canvasGroup.alpha = 1; 
        }
        else {
            _canvasGroup.blocksRaycasts = _canvasGroup.interactable = false;
            _canvasGroup.alpha = 0; 
        }
	}


    public void showNext() {

        if(nextMenu != null) { 
            GameObject.Find("intro scene").GetComponent<menuManager>().showMenu(nextMenu);
        } 
    }

}
