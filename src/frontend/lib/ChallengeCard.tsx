import Link from "next/link";
import { Challenge } from "./ClassTypes";
export function ChallengeCard({challenge}:{challenge:Challenge}){
    console.log(challenge.verified);
    console.log(challenge.viewerOwner);
    return(
        <div>
            <p>{challenge.id}</p>
            <p>{challenge.title}</p>
            <p>{challenge.username}</p>
            {challenge.viewerOwner &&(
                <div>
                    <Link href={`/challenge/${challenge.id}/edit_general`}>Edit general</Link>
                    <Link href={`/challenge/${challenge.id}/languages`}>Languages support</Link>
                </div>
            )}
            <p>___</p>
        </div>
    )
}