﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Fish : MonoBehaviour {

    private float mCurrentLife = 0;

    private float mLastFrameLife;
    
    private float mStepTime = 0;
    
    private int mCurrentStep = 0;
    
    private int mLastFrameStep = 0;
	
    private float mSpeedScaleFactor;

    [SerializeField]
    private float mSpeed = 20;
    
    private FinePoint oneFinePoint;

	private float SECOND_ONE_FRAME = 0.02f;

    [SerializeField]
    private FishPath mFishPath;

    public float Speed
    {
        get { return mSpeed; }
        set 
        { 
            mSpeed = value;
            //if (mFishPath != null)
            //{
            //    if (Application.isPlaying == false)
            //    {
            //        if (mFishPath.isNewPath == true)
            //            mFishPath.baseSpeed = mSpeed;
            //    }
            //    mSpeedScaleFactor = mSpeed / mFishPath.baseSpeed;
            //}
        }
    }

    public FishPath FishPathData
    {
        get { return mFishPath; }
        set 
        { 
            mFishPath = value;
            if (mFishPath != null)
            {
                if (Application.isPlaying == false)
                {
                    if (mFishPath.isNewPath == true)
                        mFishPath.baseSpeed = mSpeed;
                }
                mSpeedScaleFactor = mSpeed / mFishPath.baseSpeed;
            }
        }
    }

    private float mUnActiveTime = 0;
    public float UnActiveTime
    {
        get { return mUnActiveTime; }
        set { mUnActiveTime = value; }
    }

	// Use this for initialization
	void Start ()
    {
	}

    void OnDrawGizmos()
    {
        if (mFishPath && mFishPath.renderPath)
        {
            if (mFishPath.StartPosition != transform.position || mFishPath.FinePointList.Count == 0 || mFishPath.StartRotation != transform.eulerAngles)
            {
                if (Application.isPlaying == false)
                {
                    mFishPath.StartPosition = transform.position;
                    mFishPath.StartRotation = transform.eulerAngles;
                }
                mFishPath.CaculateFinePoints();
            }
            for (int i = 0; i < mFishPath.FinePointList.Count - 1; i++)
            {
                try
                {
                    if (mFishPath.controlPoints[mFishPath.FinePointList[i].controlIndex].highLight)
                        Gizmos.color = mFishPath.controlPoints[mFishPath.FinePointList[i].controlIndex].color;
                    else
                        Gizmos.color = mFishPath.lineColour;
                    Gizmos.DrawLine(mFishPath.FinePointList[i].position, mFishPath.FinePointList[i + 1].position);
                }
                catch
                {
                    //Debug.Log("wzw");
                }

            }
        }
    }

	// Update is called once per frame
	void Update () 
	{
        if (mFishPath == null) return;
        if (mUnActiveTime > 0)
        {
            transform.Translate(Vector3.forward * Time.deltaTime * mSpeed);
            mUnActiveTime -= Time.deltaTime;
            return;
        }
		float framedt = Time.deltaTime * mSpeedScaleFactor;
		mLastFrameLife = mCurrentLife;
		mCurrentLife += framedt;
		mStepTime = 0;
        if (mCurrentLife > 6)
        {
            Destroy(this.gameObject);
            return;
        }
        for (int i = 0; i < mFishPath.controlPoints.Length; i++)
		{
			mStepTime += mFishPath.controlPoints[i].mTime;
			if(mCurrentLife <= mStepTime)
			{
				mCurrentStep = i;
				break;
			}
			else
			{
				mCurrentStep = mFishPath.controlPoints.Length;
			}
		}

		if(mLastFrameStep != mCurrentStep)
		{
			int tmpStep = mLastFrameStep;
			float t1 = mLastFrameLife;
			while(true)
			{
				tmpStep = tmpStep + 1;
				if(tmpStep > mCurrentStep)
				{
					break;
				}
				float t2 = 0;
				for(int i = 0; i < tmpStep; i ++)
				{
					t2 += mFishPath.controlPoints[i].mTime;
				}
				float dt1 = t2 - t1;
				t1 = t2;
				int cnt1 = Mathf.FloorToInt(dt1 / SECOND_ONE_FRAME);
				for(int i = 0; i < cnt1; i ++)
				{
                    CaculateTransform(tmpStep - 1, SECOND_ONE_FRAME);
				}
                CaculateTransform(tmpStep - 1, dt1 - SECOND_ONE_FRAME * cnt1);
			}
			float t3 = 0;
			for(int i = 0; i < mCurrentStep; i ++)
			{
				t3 += mFishPath.controlPoints[i].mTime;
			}
			float dt2 = mCurrentLife - t3;
			int cnt2 = Mathf.FloorToInt(dt2 / SECOND_ONE_FRAME);
			for(int i = 0; i < cnt2; i ++)
			{
                CaculateTransform(mCurrentStep, SECOND_ONE_FRAME);
			}
            CaculateTransform(mCurrentStep, dt2 - SECOND_ONE_FRAME * cnt2);
			mLastFrameStep = mCurrentStep;
		}
		else
		{
			int cnt1 = Mathf.FloorToInt(framedt / SECOND_ONE_FRAME);
			for(int i = 0; i < cnt1; i ++)
			{
                CaculateTransform(mCurrentStep, SECOND_ONE_FRAME);
			}
            CaculateTransform(mCurrentStep, framedt - SECOND_ONE_FRAME * cnt1);
		}        
	}

    public void CaculateTransform(int step, float dt)
    {
        oneFinePoint = mFishPath.CaculateOneFinePoint(transform.localPosition, transform.eulerAngles, step, dt);
        transform.localPosition = oneFinePoint.position;
        transform.eulerAngles = oneFinePoint.rotation;
    }
}
