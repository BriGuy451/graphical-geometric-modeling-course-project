using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class BlockInputOverlayBehaviour : MonoBehaviour {
    
    [SerializeField] private MMF_Player m_feedbackPlayer;

    [SerializeField] private float rotationSpeed = 1.2f;

    private void Start()
    {
        List<MMF_Rotation> rotationFeedbacks = m_feedbackPlayer.GetFeedbacksOfType<MMF_Rotation>();

        foreach (MMF_Rotation rotationFeedback in rotationFeedbacks)
        {
            rotationFeedback.FeedbackDuration = rotationSpeed;
        }
    }

    private void OnEnable()
    {
        m_feedbackPlayer.PlayFeedbacks();
    }
    
    private void OnDisable()
    {
        m_feedbackPlayer.StopFeedbacks();
    }

}