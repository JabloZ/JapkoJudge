'use client';

import { useState } from "react";
import { useActionState } from "react";
import { Profile } from "@/lib/ClassTypes";
import { ChallengeCard} from "@/lib/ChallengeCard";
import { Challenge } from "@/lib/ClassTypes";
import { usePathname } from "next/navigation";
import Link from "next/link";
export function ShowProfile({username,profile}:{username:string,profile:Profile}){
    
    return(
        <div>
            <p>Name: {username}</p>
            <Link href={`/users/${username}/challenges`}>Authored challenges</Link>
            <p>Challenges solved: {profile.challenges}</p>
            <p>Points: {profile.points}</p>
        </div>
    );
}
