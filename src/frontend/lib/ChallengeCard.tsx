import Link from "next/link";
import { Challenge } from "./ClassTypes";
import { DeleteChallengeForm } from "@/app/users/[username]/challenges/ShowChallenges";
export function ChallengeCard({challenge}:{challenge:Challenge}){
    
    return(
        <div>
            <p>{challenge.id}</p>
            <p>{challenge.title}</p>
            <p>{challenge.username}</p>
            {challenge.viewerOwner &&(
                <div>
                    <Link href={`/challenge/${challenge.id}/edit_general`}>Edit general</Link>
                    <Link href={`/challenge/${challenge.id}/languages`}>Languages support</Link>
                    <DeleteChallengeForm key={'d'+challenge.id} id={challenge.id.toString()}/>
                </div>
            )}
            <p>___</p>
        </div>
    )
}