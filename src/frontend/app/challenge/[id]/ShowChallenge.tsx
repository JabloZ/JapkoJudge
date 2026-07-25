'use client';

import { useState } from "react";
import { useActionState } from "react";
import { ChallengeCard} from "@/lib/ChallengeCard";
import { Challenge } from "@/lib/ClassTypes";
import { usePathname } from "next/navigation";
import Link from "next/link";
export function ShowChallenge({challenge}:{challenge:Challenge}){
    
    return(
        <div>
            <p>Challenge #{challenge.id}: {challenge.title}</p>
            <p>Description:</p>
            <p>{challenge.description}</p>
            <Link href={`/challenge/${challenge.id}/submit_answer`}>Submit answer</Link>
        </div>
    );
}
