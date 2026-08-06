'use client';

import { useState } from "react";
import { useActionState } from "react";
import { GetChallengesRequest, PostChallengeDecision } from "./actions";
import { ChallengeCard} from "@/lib/ChallengeCard";
import { Challenge } from "@/lib/ClassTypes";
import { usePathname } from "next/navigation";

export function ShowUnverifiedChallenges({challenges}:{challenges:Challenge[]}){
    
    return(
        <div>
            {challenges.map((challenge: Challenge)=>(
                 <div key={challenge.id}>
                    {challenge.verified ? (
                    <form action={async () => { await PostChallengeDecision(challenge.id.toString(), false); }}>
                        <button type="submit">Unverify</button>
                    </form>
                    ) : (
                    <form action={async () => { await PostChallengeDecision(challenge.id.toString(), true); }}>
                    <button type="submit">Verify</button>
                    </form>
                    )}
                    <ChallengeCard challenge={challenge}/>
                </div>
            ))}
        </div>
    );
}
