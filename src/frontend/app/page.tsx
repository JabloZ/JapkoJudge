import Link from "next/link";
import { getSession } from "@/lib/session";
import { redirect } from "next/navigation";
import { logout } from "./login/actions";
export default async function Home(){
  //TODO - MIDDLEWARE THAT REDIRECTS BEFORE WEBSITE STARTS RENDERING
   const session = await getSession();

    if (!session) {
        redirect("/login");
    }

    return <div>Witaj, {session.username}!
    <div>
      <Link href={"/create_challenge"}>Create Challenge</Link>|
      <Link href={`users/${session.username}/challenges`}>Your challenges</Link>|
      <Link href={`/submissions`}>Submissions</Link>|
      <Link href={`users/${session.username}/profile`}>Your profile</Link>
      <form action={logout}>
        <button type="submit">Logout</button>
      </form>
    </div>
    </div>;

}