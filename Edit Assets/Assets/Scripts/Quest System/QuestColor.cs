using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestColor : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material QuestDefault;
    [SerializeField] private Material QuestCantStart;
    [SerializeField] private Material QuestCanStart;
    [SerializeField] private Material QuestInProgress;
    [SerializeField] private Material QuestCanFinish;
    [SerializeField] private MeshRenderer QuestRenderer;

    public void SetState(QuestState state, bool startPoint, bool finishPoint)
    {
        QuestRenderer.material = QuestDefault;
        switch (state)
        {
            case QuestState.REQUIREMENTS_NOT_MET:
                if (startPoint)
                {
                    QuestRenderer.material = QuestCantStart;
                }
                break;
            case QuestState.CAN_START:
                if (startPoint)
                {
                    QuestRenderer.material = QuestCanStart;
                }
                break;
            case QuestState.IN_PROGRESS:
                if (finishPoint)
                {
                    QuestRenderer.material = QuestInProgress;
                }
                break;
            case QuestState.CAN_FINISH:
                if (finishPoint)
                {
                    QuestRenderer.material = QuestCanFinish;
                }
                break;
            case QuestState.FINISHED:
                if (finishPoint)
                {
                    QuestRenderer.material = QuestCanStart;
                }
                break;
            default:
                QuestRenderer.material = QuestDefault;
                Debug.LogWarning("fejiwkf");
                break;
        }
    }
}
