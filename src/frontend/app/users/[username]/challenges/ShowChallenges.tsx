'use client';

import { useState } from "react";
import { useActionState } from "react";
import { GetChallengesRequest } from "./actions";
import { ChallengeCard} from "@/lib/ChallengeCard";
import { Challenge } from "@/lib/ClassTypes";
import { usePathname } from "next/navigation";
import { DeleteChallengeHandle } from "./actions";
export function ShowChallenges({username, challenges}:{username:string, challenges:Challenge[]}){
    
    return(
        <div>
            {challenges.map((challenge: Challenge)=>(
                <div>
                <ChallengeCard key={challenge.id} challenge={challenge}/>
                
                </div>
            ))}
        </div>
    );
}
export function DeleteChallengeForm({id}:{id: string}){
    const pathname = usePathname();
    const actionId=DeleteChallengeHandle.bind(null,id,pathname);
    const [state,formAction,isPending]=useActionState(actionId,null);
    
    return(
        <div>
            <form action={formAction}>
                <button type="submit">Delete</button>
            </form>
            
        </div>
    );
}