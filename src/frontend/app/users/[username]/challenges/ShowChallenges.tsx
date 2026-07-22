'use client';

import { useState } from "react";
import { useActionState } from "react";
import { GetChallengesRequest } from "./actions";
import { ChallengeCard} from "@/lib/ChallengeCard";
import { Challenge } from "@/lib/ClassTypes";
export function ShowChallenges({username, challenges}:{username:string, challenges:Challenge[]}){
    
    return(
        <div>
            {challenges.map((challenge: Challenge)=>(
                <ChallengeCard key={challenge.id} challenge={challenge}/>
            ))}
        </div>
    );
}